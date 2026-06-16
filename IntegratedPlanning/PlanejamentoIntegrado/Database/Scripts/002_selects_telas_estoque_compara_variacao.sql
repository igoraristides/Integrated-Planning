/*
  SELECTs equivalentes às consultas das telas:
  - Stock/Index.cshtml
  - Stock/Details.cshtml
  - CompareVariation/Index.cshtml
  - CompareVariation/Details.cshtml

  Parâmetros de filtro (substitua pelos valores desejados ou deixe em branco):
  - :p_supplier_name       = ''           -- código/nome do fornecedor (campo SupplierCode da tela)
  - :p_search_term         = ''           -- peça / busca (ProductCode ou Description)
  - :p_model_1             = ''           -- modelo (DESCRICAO em minúsculo na aplicação)
  - :p_model_2             = ''           -- segundo modelo, se houver
  - :p_inventory_item_id   = 12345        -- id da peça nos detalhes
  - :p_start_date          = DATE '2026-01-01'
  - :p_end_date            = DATE '2026-06-16'
  - :p_creation_start_date = DATE '2026-01-01'
  - :p_creation_end_date   = DATE '2026-06-16'
  - :p_page_number         = 1
  - :p_page_size           = 10
  - :p_customer_item_ext   = 'SEG2'       -- parte central do COD_PRODUTO (após split por '-')
  - :p_concatenated_segments = 'SEG1-SEG2-SEG3' -- COD_PRODUTO completo (consumo de material)

  Constantes usadas na aplicação:
  - ORGANIZATION_ID estoque/material consumido = 192
  - ORG_ID schedules = 83
*/

-- =============================================================================
-- STOCK / INDEX (GET) - Carregamento dos filtros
-- =============================================================================

-- 1) Fornecedores (dropdown FORNECEDOR)
SELECT
    f.COD_FORNECEDOR,
    f.NOME_FORNECEDOR,
    f.BAIRRO,
    f.CIDADE
FROM VW_M2V_PI_FORNECEDORES f
WHERE f.DT_INATIVACAO IS NULL
ORDER BY f.NOME_FORNECEDOR;


-- 2) Modelos distintos (dropdown MODELO)
SELECT DISTINCT
    s.DESCRICAO
FROM VW_M2V_ITENS_ESTOQUE_ONHAND s
WHERE s.ORGANIZATION_ID = 192
  AND s.DESCRICAO IS NOT NULL
ORDER BY s.DESCRICAO;


-- 3) Peças distintas (dropdown PEÇA)
SELECT DISTINCT
    s.COD_PRODUTO
FROM VW_M2V_ITENS_ESTOQUE_ONHAND s
WHERE s.ORGANIZATION_ID = 192
  AND s.COD_PRODUTO IS NOT NULL
ORDER BY s.COD_PRODUTO;


-- =============================================================================
-- STOCK / INDEX (POST GetData) - Grid de resultados
-- Filtros de data da tela NÃO são enviados ao backend nesta tela.
-- =============================================================================

-- 4) Total de registros filtrados
SELECT COUNT(*) AS TOTAL_REGISTROS
FROM VW_M2V_ITENS_ESTOQUE_ONHAND s
WHERE s.ORGANIZATION_ID = 192
  AND (
        :p_supplier_name IS NULL
        OR TRIM(:p_supplier_name) = ''
        OR (s.FORNECEDOR IS NOT NULL AND s.FORNECEDOR LIKE '%' || :p_supplier_name || '%')
      )
  AND (
        :p_model_1 IS NULL
        OR TRIM(:p_model_1) = ''
        OR LOWER(s.DESCRICAO) IN (LOWER(:p_model_1), LOWER(NVL(:p_model_2, :p_model_1)))
      )
  AND (
        :p_search_term IS NULL
        OR TRIM(:p_search_term) = ''
        OR (s.COD_PRODUTO IS NOT NULL AND s.COD_PRODUTO LIKE '%' || :p_search_term || '%')
        OR (s.DESCRICAO IS NOT NULL AND s.DESCRICAO LIKE '%' || :p_search_term || '%')
      );


-- 5) Dados paginados do grid
SELECT *
FROM (
    SELECT
        s.INVENTORY_ITEM_ID,
        s.COD_PRODUTO,
        s.DESCRICAO,
        s.FORNECEDOR,
        s.NEW_COST,
        s.ON_HAND,
        s.VALOR_TOTAL,
        s.ORIGEM_MERC,
        ROW_NUMBER() OVER (ORDER BY s.VALOR_TOTAL DESC) AS RN
    FROM VW_M2V_ITENS_ESTOQUE_ONHAND s
    WHERE s.ORGANIZATION_ID = 192
      AND (
            :p_supplier_name IS NULL
            OR TRIM(:p_supplier_name) = ''
            OR (s.FORNECEDOR IS NOT NULL AND s.FORNECEDOR LIKE '%' || :p_supplier_name || '%')
          )
      AND (
            :p_model_1 IS NULL
            OR TRIM(:p_model_1) = ''
            OR LOWER(s.DESCRICAO) IN (LOWER(:p_model_1), LOWER(NVL(:p_model_2, :p_model_1)))
          )
      AND (
            :p_search_term IS NULL
            OR TRIM(:p_search_term) = ''
            OR (s.COD_PRODUTO IS NOT NULL AND s.COD_PRODUTO LIKE '%' || :p_search_term || '%')
            OR (s.DESCRICAO IS NOT NULL AND s.DESCRICAO LIKE '%' || :p_search_term || '%')
          )
)
WHERE RN BETWEEN ((:p_page_number - 1) * :p_page_size + 1)
             AND (:p_page_number * :p_page_size);


-- =============================================================================
-- COMPARE VARIATION / INDEX (GET) - Mesmos filtros do Stock/Index
-- =============================================================================
-- Reutilize os SELECTs 1, 2 e 3 acima.


-- =============================================================================
-- COMPARE VARIATION / INDEX (POST GetData) - Grid + demanda/variação
-- =============================================================================

-- 6) Total de registros filtrados (igual ao Stock/Index)
-- Reutilize o SELECT 4.


-- 7) Itens paginados (igual ao Stock/Index)
-- Reutilize o SELECT 5.


-- 8) Demanda e variação por peça (executado para cada linha do grid)
-- Se as datas não forem informadas, a aplicação usa os últimos 10 períodos (~70 dias).
WITH schedules_filtrados AS (
    SELECT
        sch.HEADER_ID,
        sch.ITEM_DETAIL_QUANTITY,
        sch.START_DATE_TIME
    FROM VW_M2V_PI_SCHEDULES sch
    WHERE sch.ORG_ID = 83
      AND (
            :p_customer_item_ext IS NULL
            OR TRIM(:p_customer_item_ext) = ''
            OR UPPER(sch.CUSTOMER_ITEM_EXT) LIKE '%' || UPPER(:p_customer_item_ext) || '%'
          )
      AND (
            (:p_start_date IS NULL AND :p_end_date IS NULL)
            OR (
                sch.START_DATE_TIME >= NVL(:p_start_date, SYSDATE - 70)
                AND sch.START_DATE_TIME <= NVL(:p_end_date, SYSDATE)
               )
          )
      AND (
            (:p_creation_start_date IS NULL AND :p_creation_end_date IS NULL)
            OR (
                sch.CREATION_DATE >= NVL(:p_creation_start_date, TRUNC(SYSDATE) - 70)
                AND sch.CREATION_DATE <= NVL(:p_creation_end_date, TRUNC(SYSDATE))
               )
          )
),
totais_por_header AS (
    SELECT
        sf.HEADER_ID,
        SUM(NVL(sf.ITEM_DETAIL_QUANTITY, 0)) AS TOTAL_QUANTIDADE
    FROM schedules_filtrados sf
    WHERE sf.START_DATE_TIME IS NOT NULL
    GROUP BY sf.HEADER_ID
)
SELECT
    t.HEADER_ID,
    t.TOTAL_QUANTIDADE,
    CASE
        WHEN (SELECT MIN(tp.TOTAL_QUANTIDADE) KEEP (DENSE_RANK FIRST ORDER BY tp.HEADER_ID) FROM totais_por_header tp)
             - (SELECT MAX(tp.TOTAL_QUANTIDADE) KEEP (DENSE_RANK LAST ORDER BY tp.HEADER_ID) FROM totais_por_header tp) < 0
            THEN 'UP'
        WHEN (SELECT MIN(tp.TOTAL_QUANTIDADE) KEEP (DENSE_RANK FIRST ORDER BY tp.HEADER_ID) FROM totais_por_header tp)
             - (SELECT MAX(tp.TOTAL_QUANTIDADE) KEEP (DENSE_RANK LAST ORDER BY tp.HEADER_ID) FROM totais_por_header tp) > 0
            THEN 'DOWN'
        ELSE NULL
    END AS VARIACAO,
    (SELECT MAX(tp.TOTAL_QUANTIDADE) KEEP (DENSE_RANK LAST ORDER BY tp.HEADER_ID) FROM totais_por_header tp) AS ULTIMA_DEMANDA
FROM totais_por_header t
ORDER BY t.HEADER_ID;


-- =============================================================================
-- STOCK / DETAILS e COMPARE VARIATION / DETAILS
-- =============================================================================

-- 9) Dados da peça selecionada
SELECT
    s.INVENTORY_ITEM_ID,
    s.COD_PRODUTO,
    s.DESCRICAO,
    s.FORNECEDOR,
    s.NEW_COST,
    s.ON_HAND,
    s.VALOR_TOTAL,
    s.ORIGEM_MERC
FROM VW_M2V_ITENS_ESTOQUE_ONHAND s
WHERE s.ORGANIZATION_ID = 192
  AND s.INVENTORY_ITEM_ID = :p_inventory_item_id
  AND (
        :p_supplier_name IS NULL
        OR TRIM(:p_supplier_name) = ''
        OR (s.FORNECEDOR IS NOT NULL AND s.FORNECEDOR LIKE '%' || :p_supplier_name || '%')
      )
  AND (
        :p_model_1 IS NULL
        OR TRIM(:p_model_1) = ''
        OR LOWER(s.DESCRICAO) IN (LOWER(:p_model_1), LOWER(NVL(:p_model_2, :p_model_1)))
      )
  AND (
        :p_search_term IS NULL
        OR TRIM(:p_search_term) = ''
        OR (s.COD_PRODUTO IS NOT NULL AND s.COD_PRODUTO LIKE '%' || :p_search_term || '%')
        OR (s.DESCRICAO IS NOT NULL AND s.DESCRICAO LIKE '%' || :p_search_term || '%')
      );


-- 10) Gráfico - demanda por arquivo/período (ChartData)
-- Usado em Stock/Details e CompareVariation/Details
SELECT
    sch.HEADER_ID,
    MIN(sch.CREATION_DATE) AS CREATION_DATE,
    sch.HEADER_ID || ' - ' || TO_CHAR(MIN(sch.CREATION_DATE), 'DD/MM/YYYY') AS ROTULO_GRAFICO,
    SUM(NVL(sch.ITEM_DETAIL_QUANTITY, 0)) AS QUANTIDADE
FROM VW_M2V_PI_SCHEDULES sch
WHERE sch.ORG_ID = 83
  AND (
        :p_customer_item_ext IS NULL
        OR TRIM(:p_customer_item_ext) = ''
        OR UPPER(sch.CUSTOMER_ITEM_EXT) LIKE '%' || UPPER(:p_customer_item_ext) || '%'
      )
  AND (
        (:p_start_date IS NULL AND :p_end_date IS NULL)
        OR (
            sch.START_DATE_TIME >= NVL(:p_start_date, SYSDATE - 70)
            AND sch.START_DATE_TIME <= NVL(:p_end_date, SYSDATE)
           )
      )
  AND (
        (:p_creation_start_date IS NULL AND :p_creation_end_date IS NULL)
        OR (
            sch.CREATION_DATE >= NVL(:p_creation_start_date, TRUNC(SYSDATE) - 70)
            AND sch.CREATION_DATE <= NVL(:p_creation_end_date, TRUNC(SYSDATE))
           )
      )
GROUP BY sch.HEADER_ID
ORDER BY sch.HEADER_ID;


-- 11) Matriz de quantidade por semana (QuantityMatrix)
-- Usado em CompareVariation/Details e como base em Stock/Details
SELECT
    sch.HEADER_ID,
    MIN(sch.CREATION_DATE) AS CREATION_DATE,
    sch.HEADER_ID || ' - ' || TO_CHAR(MIN(sch.CREATION_DATE), 'DD/MM/YYYY') AS ORIGEM,
    TO_CHAR(sch.START_DATE_TIME, 'YY') || LPAD(TO_CHAR(sch.START_DATE_TIME, 'IW'), 2, '0') AS SEMANA_ISO,
    SUM(NVL(sch.ITEM_DETAIL_QUANTITY, 0)) AS QUANTIDADE_SEMANA,
    SUM(SUM(NVL(sch.ITEM_DETAIL_QUANTITY, 0))) OVER (PARTITION BY sch.HEADER_ID) AS TOTAL_HEADER
FROM VW_M2V_PI_SCHEDULES sch
WHERE sch.ORG_ID = 83
  AND sch.START_DATE_TIME IS NOT NULL
  AND (
        :p_customer_item_ext IS NULL
        OR TRIM(:p_customer_item_ext) = ''
        OR UPPER(sch.CUSTOMER_ITEM_EXT) LIKE '%' || UPPER(:p_customer_item_ext) || '%'
      )
  AND (
        (:p_start_date IS NULL AND :p_end_date IS NULL)
        OR (
            sch.START_DATE_TIME >= NVL(:p_start_date, SYSDATE - 70)
            AND sch.START_DATE_TIME <= NVL(:p_end_date, SYSDATE)
           )
      )
  AND (
        (:p_creation_start_date IS NULL AND :p_creation_end_date IS NULL)
        OR (
            sch.CREATION_DATE >= NVL(:p_creation_start_date, TRUNC(SYSDATE) - 70)
            AND sch.CREATION_DATE <= NVL(:p_creation_end_date, TRUNC(SYSDATE))
           )
      )
GROUP BY
    sch.HEADER_ID,
    TO_CHAR(sch.START_DATE_TIME, 'YY') || LPAD(TO_CHAR(sch.START_DATE_TIME, 'IW'), 2, '0')
ORDER BY sch.HEADER_ID, SEMANA_ISO;


-- 12) Matriz de valores (ValueMatrix)
-- Mesma consulta da matriz de quantidade; na aplicação multiplica por NEW_COST da peça
SELECT
    sch.HEADER_ID,
    TO_CHAR(sch.START_DATE_TIME, 'YY') || LPAD(TO_CHAR(sch.START_DATE_TIME, 'IW'), 2, '0') AS SEMANA_ISO,
    SUM(NVL(sch.ITEM_DETAIL_QUANTITY, 0)) * NVL(:p_unit_value, 35) AS VALOR_SEMANA,
    SUM(SUM(NVL(sch.ITEM_DETAIL_QUANTITY, 0))) OVER (PARTITION BY sch.HEADER_ID) * NVL(:p_unit_value, 35) AS TOTAL_HEADER
FROM VW_M2V_PI_SCHEDULES sch
WHERE sch.ORG_ID = 83
  AND sch.START_DATE_TIME IS NOT NULL
  AND (
        :p_customer_item_ext IS NULL
        OR TRIM(:p_customer_item_ext) = ''
        OR UPPER(sch.CUSTOMER_ITEM_EXT) LIKE '%' || UPPER(:p_customer_item_ext) || '%'
      )
  AND (
        (:p_start_date IS NULL AND :p_end_date IS NULL)
        OR (
            sch.START_DATE_TIME >= NVL(:p_start_date, SYSDATE - 70)
            AND sch.START_DATE_TIME <= NVL(:p_end_date, SYSDATE)
           )
      )
  AND (
        (:p_creation_start_date IS NULL AND :p_creation_end_date IS NULL)
        OR (
            sch.CREATION_DATE >= NVL(:p_creation_start_date, TRUNC(SYSDATE) - 70)
            AND sch.CREATION_DATE <= NVL(:p_creation_end_date, TRUNC(SYSDATE))
           )
      )
GROUP BY
    sch.HEADER_ID,
    TO_CHAR(sch.START_DATE_TIME, 'YY') || LPAD(TO_CHAR(sch.START_DATE_TIME, 'IW'), 2, '0')
ORDER BY sch.HEADER_ID, SEMANA_ISO;


-- 13) Consumo de material por semana (apenas Stock/Details)
SELECT
    TO_CHAR(mc.TRANSACTION_DATE, 'YY') || LPAD(TO_CHAR(mc.TRANSACTION_DATE, 'IW'), 2, '0') AS SEMANA_ISO,
    SUM(NVL(mc.TRANSACTION_QUANTITY, 0)) AS QUANTIDADE_CONSUMIDA
FROM VW_M2V_PI_MATERIAL_CONSUMIDO mc
WHERE mc.ORGANIZATION_ID = 192
  AND mc.CONCATENATED_SEGMENTS = :p_concatenated_segments
  AND mc.TRANSACTION_DATE IS NOT NULL
  AND TRUNC(mc.TRANSACTION_DATE) >= NVL(:p_creation_start_date, TRUNC(SYSDATE) - 70)
  AND TRUNC(mc.TRANSACTION_DATE) <= NVL(:p_creation_end_date, TRUNC(SYSDATE))
GROUP BY TO_CHAR(mc.TRANSACTION_DATE, 'YY') || LPAD(TO_CHAR(mc.TRANSACTION_DATE, 'IW'), 2, '0')
ORDER BY SEMANA_ISO;


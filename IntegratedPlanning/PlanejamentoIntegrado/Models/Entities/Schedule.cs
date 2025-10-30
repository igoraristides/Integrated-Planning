namespace PlanejamentoIntegrado.Models;

public class Schedule
{
    /// <summary>HEADER_ID - Identificador único do cabeçalho do agendamento</summary>
    public int HeaderId { get; set; }

    /// <summary>ORG_ID - Identificador da organização</summary>
    public int OrganizationId { get; set; }

    /// <summary>LINE_ID - Identificador único da linha do agendamento</summary>
    public int LineId { get; set; }

    /// <summary>INTERFACE_HEADER_ID - ID do cabeçalho da interface externa</summary>
    public int? InterfaceHeaderId { get; set; }

    /// <summary>INTERFACE_LINE_ID - ID da linha da interface externa</summary>
    public int? InterfaceLineId { get; set; }

    /// <summary>CUSTOMER_SHIP_TO_EXT - Código do destinatário do cliente</summary>
    public string? CustomerShipToExt { get; set; }

    /// <summary>INDUSTRY_ATTRIBUTE10 - Atributo customizado da indústria 10</summary>
    public string? IndustryAttribute10 { get; set; }

    /// <summary>INDUSTRY_ATTRIBUTE3 - Atributo customizado da indústria 3</summary>
    public string? IndustryAttribute3 { get; set; }

    /// <summary>CUSTOMER_ID - Identificador do cliente</summary>
    public int CustomerId { get; set; }

    /// <summary>SCHEDULE_TYPE - Tipo do agendamento</summary>
    public string? ScheduleType { get; set; }

    /// <summary>SCHEDULE_HORIZON_START_DATE - Data de início do horizonte de agendamento</summary>
    public DateTime? ScheduleHorizonStartDate { get; set; }

    /// <summary>SCHEDULE_HORIZON_END_DATE - Data de fim do horizonte de agendamento</summary>
    public DateTime? ScheduleHorizonEndDate { get; set; }

    /// <summary>SCHEDULE_REFERENCE_NUM - Número de referência do agendamento</summary>
    public string? ScheduleReferenceNum { get; set; }

    /// <summary>CUSTOMER_NAME_EXT - Nome externo do cliente</summary>
    public string? CustomerNameExt { get; set; }

    /// <summary>SCHEDULE_GENERATION_DATE - Data de geração do agendamento</summary>
    public DateTime? ScheduleGenerationDate { get; set; }

    /// <summary>LINE_NUMBER - Número da linha</summary>
    public int? LineNumber { get; set; }

    /// <summary>CREATION_DATE - Data de criação do registro</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>START_DATE_TIME - Data e hora de início programada</summary>
    public DateTime? StartDateTime { get; set; }

    /// <summary>CUSTOMER_ITEM_EXT - Código do item do cliente</summary>
    public string? CustomerItemExt { get; set; }

    /// <summary>CUSTOMER_ITEM_ID - Identificador do item do cliente</summary>
    public int CustomerItemId { get; set; }

    /// <summary>INVENTORY_ITEM_ID - Identificador do item no inventário</summary>
    public int InventoryItemId { get; set; }

    /// <summary>ITEM_DETAIL_QUANTITY - Quantidade detalhada do item</summary>
    public decimal? ItemDetailQuantity { get; set; }

    /// <summary>UOM_CODE - Código da unidade de medida</summary>
    public string? UomCode { get; set; }

    /// <summary>SHIP_TO_ORG_ID - ID da organização de destino</summary>
    public int? ShipToOrgId { get; set; }

    /// <summary>SHIP_FROM_ORG_ID - ID da organização de origem</summary>
    public int? ShipFromOrgId { get; set; }

    /// <summary>SHIP_TO_ADDRESS_ID - ID do endereço de entrega</summary>
    public int? ShipToAddressId { get; set; }

    /// <summary>SHIP_TO_NAME_EXT - Nome externo do destinatário</summary>
    public string? ShipToNameExt { get; set; }

    /// <summary>PROCESS_STATUS - Status do processamento</summary>
    public int? ProcessStatus { get; set; }
}

Imports MerchSys.Accounting.Entities

Namespace Services
    Public Interface IVatReturnExporter
        Function ExportCsv(vatReturn As VatReturn) As Byte()
        Function ExportPdf(vatReturn As VatReturn) As Byte()
    End Interface
End Namespace

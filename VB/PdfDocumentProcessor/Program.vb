Imports System
Imports DevExpress.Pdf
Imports System.Diagnostics
Imports DevExpress.Office.DigitalSignatures
Imports DevExpress.Office.Tsp

Namespace PdfDocumentProcessor

    Friend Class Program

        Shared Sub Main(ByVal args As String())
            Call ApplySignatures()
            Call Process.Start("SignedDocument.pdf")
        End Sub

        Public Shared Sub ApplySignatures()
            'Load a document to sign
            Using signer = New PdfDocumentSigner("Document.pdf")
                'Specify the name and location of the signature field
                Dim signatureFieldInfo = New PdfSignatureFieldInfo(1)
                signatureFieldInfo.Name = "SignatureField"
                signatureFieldInfo.SignatureBounds = New PdfRectangle(20, 20, 150, 150)
                signatureFieldInfo.RotationAngle = PdfAcroFormFieldRotation.Rotate90
                'Create a timestamp
                Dim tsaClient As ITsaClient = New TsaClient(New Uri("https://freetsa.org/tsr"), HashAlgorithmType.SHA256)
                'Create a PAdES PKCS#7 signature
                Dim pkcs7Signature As Pkcs7Signer = New Pkcs7Signer("Signing Documents/certificate.pfx", "123", HashAlgorithmType.SHA256, tsaClient, Nothing, Nothing, PdfSignatureProfile.PAdES_BES)
                'Apply a signature to a new form field created before
                Dim cooperSignature = New PdfSignatureBuilder(pkcs7Signature, signatureFieldInfo)
                'Specify an image and signer information
                cooperSignature.SetImageData(IO.File.ReadAllBytes("Signing Documents/JaneCooper.jpg"))
                cooperSignature.Location = "USA"
                cooperSignature.Name = "Jane Cooper"
                cooperSignature.Reason = "Acknowledgement"
                'Apply a signature to an existing form field
                Dim santuzzaSignature = New PdfSignatureBuilder(pkcs7Signature, "Sign")
                'Specify an image and signer information
                santuzzaSignature.Location = "Australia"
                santuzzaSignature.Name = "Santuzza Valentina"
                santuzzaSignature.Reason = "I Agree"
                ' Specify signature appearance parameters:
                Dim signatureAppearance As PdfSignatureAppearance = New PdfSignatureAppearance()
                signatureAppearance.SetImageData("Signing Documents/SantuzzaValentina.png")
                signatureAppearance.SignatureDetailsFont.Size = 12
                signatureAppearance.SignatureDetailsFont.Italic = True
                signatureAppearance.ShowDate = True
                signatureAppearance.ShowReason = True
                santuzzaSignature.SetSignatureAppearance(signatureAppearance)
                'Create a new signature form field:
                Dim signatureFieldInfo1 = New PdfSignatureFieldInfo(1)
                signatureFieldInfo1.Name = "SignatureField1"
                signatureFieldInfo1.SignatureBounds = New PdfRectangle(200, 200, 250, 250)
                'Create a document level time stamp:
                Dim pdfTimeStamp As PdfTimeStamp = New PdfTimeStamp(tsaClient)
                'Apply this time stamp to the form field:
                Dim timeStampSignature = New PdfSignatureBuilder(pdfTimeStamp, signatureFieldInfo1)
                'Add signatures to an array
                Dim signatures As PdfSignatureBuilder() = {cooperSignature, santuzzaSignature, timeStampSignature}
                'Sign and save the document
                signer.SaveDocument("SignedDocument.pdf", signatures)
            End Using
        End Sub
    End Class
End Namespace

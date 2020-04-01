using System;
using DevExpress.Pdf;
using System.Diagnostics;
using System.Linq;

namespace PdfDocumentProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplySignatures();
            Process.Start("SignedDocument.pdf");
        }

        public static void ApplySignatures()
        {
            //Load a document to sign
            using (var signer = new PdfDocumentSigner("Document.pdf"))
            {
                //Specify the name and location of the signature field
                var signatureFieldInfo = new PdfSignatureFieldInfo(1);
                signatureFieldInfo.Name = "SignatureField";
                signatureFieldInfo.SignatureBounds = new PdfRectangle(20, 20, 150, 150);
                signatureFieldInfo.RotationAngle = PdfAcroFormFieldRotation.Rotate90;
                
                //Create a timestamp
                ITsaClient tsaClient = new PdfTsaClient(new Uri(@"https://freetsa.org/tsr"), PdfHashAlgorithm.SHA256);

                //Create a PKCS#7 signature
                Pkcs7Signer pkcs7Signature = new Pkcs7Signer("Signing Documents//certificate.pfx", "123", PdfHashAlgorithm.SHA256, tsaClient);

                //Apply a signature to a new form field created before
                var cooperSignature = new PdfSignatureBuilder(pkcs7Signature, signatureFieldInfo);

                //Specify an image and signer information
                cooperSignature.SetImageData(System.IO.File.ReadAllBytes("Signing Documents//JaneCooper.jpg"));
                cooperSignature.Location = "USA";
                cooperSignature.Name = "Jane Cooper";
                cooperSignature.Reason = "Acknowledgement";

                //Apply a signature to an existing form field
                var santuzzaSignature = new PdfSignatureBuilder(pkcs7Signature, "Sign");

                //Specify an image and signer information
                santuzzaSignature.SetImageData(System.IO.File.ReadAllBytes("Signing Documents//SantuzzaValentina.jpg"));
                santuzzaSignature.Location = "Australia";
                santuzzaSignature.Name = "Santuzza Valentina";
                santuzzaSignature.Reason = "I Agree";

                //Add signatures to an array
                PdfSignatureBuilder[] signatures = { cooperSignature, santuzzaSignature };

                //Sign and save the document
                signer.SaveDocument("SignedDocument.pdf", signatures);
            }
        }
    }
}


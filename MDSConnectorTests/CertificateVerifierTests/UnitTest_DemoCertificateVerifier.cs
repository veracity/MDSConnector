using Microsoft.VisualStudio.TestTools.UnitTesting;
using MDSConnector.Utilities;
using System.Security.Cryptography.X509Certificates;
using MDSConnectorTests.Utilities;
using Moq;
using System;
using System.Collections.Generic;

namespace MDSConnectorTests
{
    [TestClass]
    public class UnitTest_DemoCertificateVerifier
    {
        [TestMethod]
        public void TestDemoCertificateVerifier_noCertificate_returnFalse()
        {
            //Arrange
            X509Certificate2 certificate = null;
            DemoCertificateVerifier verifier = new DemoCertificateVerifier();

            //Act
            var verificationResult = verifier.verify(certificate, new HashSet<string>());

            //Assert
            Assert.AreEqual(false, verificationResult.valid);
            Assert.AreEqual("No certificate provided", verificationResult.reason);
        }

        [TestMethod]
        public void TestDemoCertificateVerifier_IssuerSubjectMissMatch_returnFalse()
        {
            //Arrange
            DateTime today = DateTime.Today;
            DateTime twoHoursFromNow = today.AddHours(2);

            X509Certificate2 certificate = CertificateGenerator.
                Build("issuer@email.com", 
                        "notIssuer@email.com", 
                        today, 
                        twoHoursFromNow, 
                        2048
                    );
            
            DemoCertificateVerifier verifier = new DemoCertificateVerifier();

            //Act
            var verificationResult = verifier.verify(certificate, new HashSet<string>());

            //Assert
            Assert.AreEqual(false, verificationResult.valid);
            Assert.AreEqual("Issuer and subject miss match", verificationResult.reason);
        }

        [TestMethod]
        public void TestDemoCertificateVerifier_IssuerGoogleSubjectSomeElse_returnFalse()
        {
            //Arrange
            DateTime today = DateTime.Today;
            DateTime twoHoursFromNow = today.AddHours(2);

            X509Certificate2 certificate = CertificateGenerator.
                Build("google.com",
                        "notGoogle@email.com",
                        today,
                        twoHoursFromNow,
                        2048
                    );

            DemoCertificateVerifier verifier = new DemoCertificateVerifier();

            //Act
            var verificationResult = verifier.verify(certificate, new HashSet<string>());

            //Assert
            Assert.AreEqual(false, verificationResult.valid);
            Assert.AreEqual("Google", verificationResult.reason);
        }
    }
}

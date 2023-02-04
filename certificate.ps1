$cert = New-SelfSignedCertificate -NotAfter $(Get-Date).AddYears(1) -FriendlyName "MyDevServerCertificate" -CertStoreLocation cert:\CurrentUser\My -Subject "CN=MyDevServerCertificate" -KeyAlgorithm RSA -KeyLength 2048 -KeyExportPolicy NonExportable
Export-Certificate -Cert $cert -Type cer -FilePath "$PWD/certificate.cer" -Force
$cert | Format-list
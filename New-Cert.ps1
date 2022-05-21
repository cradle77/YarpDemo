# run this as admin

$rootCert = New-SelfSignedCertificate -DnsName "localhost", "localhost" 
  -CertStoreLocation "cert:\LocalMachine\My" 
  -NotAfter (Get-Date).AddYears(20) 
  -FriendlyName "YarpDemoRoot" 
  -KeyUsageProperty All 
  -KeyUsage CertSign, CRLSign, DigitalSignature

$rootThumbprint = $rootCert.Thumbprint

$mypwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText

Get-ChildItem -Path "cert:\localMachine\my\$rootThumbprint" | Export-PfxCertificate -FilePath .\YarpDemoRoot.pfx -Password $mypwd

$clientCert = New-SelfSignedCertificate 
  -certstorelocation cert:\localmachine\my 
  -dnsname "localhost" 
  -Signer $rootCert 
  -NotAfter (Get-Date).AddYears(20) 
  -FriendlyName "YarpDemoClient"
 
$clientThumbprint = $clientCert.Thumbprint

Get-ChildItem -Path "cert:\localMachine\my\$clientThumbprint" | Export-PfxCertificate -FilePath .\YarpDemoClient.pfx -Password $mypwd

Write-Host "Thumbprint: $clientThumbprint"
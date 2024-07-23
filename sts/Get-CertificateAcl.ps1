# _____________________________________________________________________________
#
# GET-CERTIFICATEACL.PS1
# _____________________________________________________________________________
# Purpose:
#	Returns the access control list (ACL) for a certificate.
#
# Author:
#	Chris Martinez
#   chrimart@microsoft.com
#
# Last Modified:
#	03/12/2009 - Created.
#
# Parameters:
#   $Certificate - The certificate to retrieve the ACL for
#	$Expand	     - Indiates whether the ACL is expanded into access rules

Param
(
 [Security.Cryptography.X509Certificates.X509Certificate2] $Certificate
,[Switch] $Expand
)

# validate arguments
if ( $Certificate -eq $null ) {
	throw (New-Object ArgumentNullException -ArgumentList "`$Certificate")
}

$csp = [Security.Cryptography.ICspAsymmetricAlgorithm] $certificate.PrivateKey

if ( $csp -eq $null ) {
	throw (New-Object InvalidOperationException -ArgumentList "The certificate key provider must support the Microsoft Cryptographic API (CAPI)." )
}

$keyInfo = $csp.CspKeyContainerInfo
$filePath = ""

# get the physical path for the certificate
if ( $keyInfo.MachineKeyStore )
{
	$filePath = [Environment]::GetFolderPath( [Environment+SpecialFolder]::CommonApplicationData )
	$filePath = Join-Path (Join-Path $filePath "Microsoft\Crypto\RSA\MachineKeys") $keyInfo.UniqueKeyContainerName
}
else
{
	$filePath = [Environment]::GetFolderPath( [Environment+SpecialFolder]::ApplicationData )
	$filePath = Join-Path (Join-Path $filePath "Microsoft\Crypto\RSA") $keyInfo.UniqueKeyContainerName
}

if ( !(Test-Path $filePath) ) {
	throw (New-Object IO.FileNotFoundException -ArgumentList "The certificate file '$filePath' does not exist.")
}

$acl = Get-Acl $filePath

# expand the acl if requested
if ( $Expand ) {
	$acl.GetAccessRules( $true, $true, [Security.Principal.NTAccount] )
} else {
	$acl
}


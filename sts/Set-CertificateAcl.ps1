# _____________________________________________________________________________
#
# SET-CERTIFICATEACL.PS1
# _____________________________________________________________________________
# Purpose:
#	Grants or revokes read permission to a certificate.
#
# Author:
#	Chris Martinez
#   chrimart@microsoft.com
#
# Last Modified:
#	03/12/2009 - Created.
#
# Parameters:
#   $Certificate - The certificate to grant or revoke permission to.
#	$AccountName - The name of the Windows NT account to grant or revoke permission
#	$Grant		 - Indicates that permission is granted to the certificate
#	$Revoke		 - Indicates that permission is revoked from the certificate

Param
(
 [Security.Cryptography.X509Certificates.X509Certificate2] $Certificate
,[String] $AccountName
,[Switch] $Grant
,[Switch] $Revoke
)

# validate arguments
if ( $Certificate -eq $null ) {
	throw (New-Object ArgumentNullException -ArgumentList "`$Certificate")
} elseif ( [String]::IsNullOrEmpty( $AccountName ) ) {
	throw (New-Object ArgumentNullException -ArgumentList "`$AccountName")
} elseif ( $Grant.IsPresent -and $Revoke.IsPresent ) {
	throw (New-Object ArgumentException -ArgumentList "The `$Grant and `$Revoke arguments cannot both be specified.")
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

$account = New-Object Security.Principal.NTAccount -ArgumentList $AccountName
$acl = Get-Acl $filePath

# update the certificate acl
if ( $Grant )
{
	$read = [Security.AccessControl.FileSystemRights]::Read
	$allow = [Security.AccessControl.AccessControlType]::Allow
	$rule = New-Object Security.AccessControl.FileSystemAccessRule -ArgumentList $account, $read, $allow
	$acl.AddAccessRule( $rule )
}
elseif ( $Revoke )
{
	$acl.PurgeAccessRules( $account )
}

Set-Acl $filePath $acl
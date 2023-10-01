#  Work Unit Downloader
#
#  Do not forget to run "Set-ExecutionPolicy Unrestricted" before running the script
#
#  To change the execution
#  policy for the default (LocalMachine) scope, start Windows PowerShell with the "Run as administrator" option. To change the execution policy for the current user, 
#  run "Set -ExecutionPolicy -Scope CurrentUser".
#
# Usage: ./wud.ps1 [workunit number] [filename] [target platform]
#
# Workunit like:   '136557899'
# filename like:   'ps_200914_input_95860_5' ( not ps_200914_input_95860_5_0 or other)
# Platform like:   'cuda102'
#
# Example:  ./wud.ps1 136601339 ps_200914_input_95860_5 cuda102
#
# File name can be obtained from http://asteroidsathome.net/boinc/workunit.php?wuid=xxxxxxxxxx
#
# Developed by Georgi Vidinski | 2019
#

$workunit = $args[0]
$file = $args[1]
$type = $args[2]
$outputfolder = "D:\Development\AsteroidsAtHome\boinc\ISSUES_DownloadedTasks\Error_in_${type}"
FOR($i = 0; $i -le 2048; $i++)
{
    $hex = [Convert]::ToString($i, 16);
    try
    {
        #$file = "ps_200306_input_6459_11"
        $uri = "http://asteroidsathome.net/boinc/download/"+$hex+"/" + $file
        Write-Host –NoNewLine  "${uri}   "

        $response = Invoke-WebRequest -Uri $uri -OutFile "${outputfolder}/${file}" -ErrorAction Stop
        
        # This will only execute if the Invoke-WebRequest is successful.
        $StatusCode = $Response.StatusCode
    }
    catch
    {
        $StatusCode = $_.Exception.Response.StatusCode.value__
    }

    if($StatusCode -ne 404){
        Write-Host –NoNewLine "${hex}: ${StatusCode}" -ForegroundColor “Green”
        Write-Host "" 
        
        $logfile = "${outputfolder}/${file}.log"
        Get-Date -Format "dd/MM/yyyy HH:mm:ss K" | Out-File -FilePath $logfile -Append
        "Workunit: ${workunit}" | Out-File -FilePath $logfile -Append
        "Stat Uri: http://asteroidsathome.net/boinc/workunit.php?wuid=${workunit}" | Out-File -FilePath $logfile -Append
        "Uri: $uri" | Out-File -FilePath $logfile -Append
        "----------------------" | Out-File -FilePath $logfile -Append
     
        Break     
    }
    
    Write-Host –NoNewLine "${hex}: ${StatusCode}" -ForegroundColor “Red”
    Write-Host ""

    
}
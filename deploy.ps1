# ------------------------------------------------------------
# Script deploy.ps1 – Deploy backend .NET API (Publish to temp folder then copy to IIS)
# ------------------------------------------------------------

# ========================
# Hàm log với timestamp
# ========================
function Write-Log($message) {
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Output "[$timestamp] $message"
}

# ========================
# Import IIS Module (yêu cầu Run as Administrator)
# ========================
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Log "✅ Imported WebAdministration module successfully."
}
catch {
    Write-Log "❌ Failed to import WebAdministration. Please run PowerShell as Administrator."
    exit 1
}

# ========================
# Thông tin cấu hình
# ========================
$repoPath = "F:\ThucTap\TaskManagementAPI"
$projectFile = "QuanLyCongViecAPI\TaskManagementAPI.csproj"
$tempPublishPath = "F:\ThucTap\TempPublish"
$deployPath = "F:\ThucTap\TASKAPI"
$appPoolName = "TaskAPI"
$backupPath = "F:\ThucTap\Backup\TASKAPI_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

# ========================
# Đi tới thư mục project
# ========================
Set-Location $repoPath

# ========================
# Pull code mới nhất
# ========================
Write-Log "🔄 Pulling latest code from GitHub..."
git pull origin master

# ========================
# Restore, Build, Publish
# ========================
Write-Log "📦 Restoring NuGet packages..."
dotnet restore $projectFile -v m

Write-Log "🏗 Building project in Release mode..."
dotnet build $projectFile -c Release -v m

Write-Log "🚀 Publishing project to temporary folder..."
dotnet publish $projectFile -c Release -o $tempPublishPath -v m

# ========================
# Stop App Pool trước khi copy đè
# ========================
Write-Log "🛑 Stopping IIS App Pool before copying files..."
try {
    Stop-WebAppPool -Name $appPoolName -ErrorAction Stop
    Write-Log "✅ App Pool stopped."
}
catch {
    Write-Log "⚠️ App Pool was already stopped or failed to stop gracefully."
}

# Chờ App Pool stop hoàn toàn (max 10s)
Write-Log "⏳ Waiting for App Pool to stop completely..."
for ($i = 1; $i -le 10; $i++) {
    $status = (Get-WebAppPoolState -Name $appPoolName).Value
    Write-Log "➡️ App Pool status: $status (try $i)"
    if ($status -eq "Stopped") { break }
    Start-Sleep -Seconds 1
}

if ($status -ne "Stopped") {
    Write-Log "⚠️ Warning: App Pool did not stop completely after 10 seconds."
}

# ========================
# Backup thư mục deploy trước khi copy
# ========================
if (Test-Path $deployPath) {
    Write-Log "💾 Backing up current deploy folder to $backupPath"
    Copy-Item -Path $deployPath -Destination $backupPath -Recurse -Force
}

# ========================
# Copy publish output sang deployPath
# ========================
Write-Log "📂 Copying published files to IIS folder..."
Copy-Item -Path "$tempPublishPath\*" -Destination $deployPath -Recurse -Force -ErrorAction Stop

# ========================
# Start lại App Pool
# ========================
Write-Log "✅ Starting IIS App Pool..."
Start-WebAppPool -Name $appPoolName -ErrorAction Stop

# Kiểm tra status sau khi start
$status = (Get-WebAppPoolState -Name $appPoolName).Value
Write-Log "➡️ App Pool current status: $status"

# ========================
# Xóa temp publish folder
# ========================
if (Test-Path $tempPublishPath) {
    Write-Log "🧹 Cleaning up temporary publish folder..."
    Remove-Item -Path $tempPublishPath -Recurse -Force
}

# ========================
# Thông báo hoàn tất
# ========================
Write-Log "🎉 Deploy completed successfully."
exit 0

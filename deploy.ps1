# ------------------------------------------------------------
# Script deploy.ps1 – Deploy backend .NET API (Publish to temp folder then copy to IIS)
# ------------------------------------------------------------

Import-Module WebAdministration

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
# Hàm log với timestamp
# ========================
function Write-Log($message) {
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Output "[$timestamp] $message"
}

# ========================
# Đi tới thư mục code git clone của dự án
# ========================
cd $repoPath

Write-Log "🔄 Pulling latest code from GitHub..."
git pull origin master

Write-Log "📦 Restoring NuGet packages..."
dotnet restore $projectFile

Write-Log "🏗 Building project in Release mode..."
dotnet build $projectFile -c Release

Write-Log "🚀 Publishing project to temporary folder..."
dotnet publish $projectFile -c Release -o $tempPublishPath

# ========================
# Stop App Pool trước khi copy đè
# ========================
Write-Log "🛑 Stopping IIS App Pool before copying files..."
try {
    Stop-WebAppPool -Name $appPoolName -ErrorAction Stop
}
catch {
    Write-Log "⚠️ App Pool was already stopped or failed to stop gracefully."
}

# Chờ App Pool stop hoàn toàn
Write-Log "⏳ Waiting for App Pool to stop completely..."
$try = 0
do {
    Start-Sleep -Seconds 1
    $status = (Get-WebAppPoolState -Name $appPoolName).Value
    $try++
    Write-Log "➡️ App Pool status: $status (try $try)"
} while ($status -ne "Stopped" -and $try -lt 10)

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
# Copy đè thư mục publish output sang deployPath
# ========================
Write-Log "📂 Copying published files to IIS folder..."
Copy-Item -Path "$tempPublishPath\*" -Destination $deployPath -Recurse -Force

# ========================
# Start lại App Pool
# ========================
Write-Log "✅ Starting IIS App Pool..."
Start-WebAppPool -Name $appPoolName

# Chờ xác nhận App Pool running
$status = (Get-WebAppPoolState -Name $appPoolName).Value
Write-Log "➡️ App Pool current status: $status"

# ========================
# Xóa temp publish folder
# ========================
Write-Log "🧹 Cleaning up temporary publish folder..."
Remove-Item -Path $tempPublishPath -Recurse -Force

# ========================
# Thông báo hoàn tất
# ========================
Write-Log "🎉 Deploy completed successfully."

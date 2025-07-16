# ------------------------------------------------------------
# Script deploy.ps1 – Deploy backend .NET API
# ------------------------------------------------------------

# Đi tới thư mục code git clone của dự án
cd "F:\ThucTap\TaskManagementAPI"

# Hiển thị log Pull code
Write-Output "🔄 Pulling latest code from GitHub..."
git pull origin master

# Restore NuGet packages (các thư viện cần thiết)
Write-Output "📦 Restoring NuGet packages..."
dotnet restore TaskManagementAPI.sln

# Build solution với cấu hình Release
Write-Output "🏗 Building project in Release mode..."
dotnet build TaskManagementAPI.sln -c Release

# Publish build output ra thư mục IIS đang trỏ tới
Write-Output "🚀 Publishing project to IIS folder..."
dotnet publish TaskManagementAPI.sln -c Release -o "F:\ThucTap\TASKAPI"

# Restart IIS App Pool để nhận code mới
Write-Output "♻️ Restarting IIS App Pool..."
Restart-WebAppPool -Name "TaskAPI"

# Thông báo hoàn tất
Write-Output "✅ Deploy completed successfully."

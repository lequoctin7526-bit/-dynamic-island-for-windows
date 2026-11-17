# Dynamic Island for Windows (Liquid Glass Desktop App)

Ứng dụng độc lập Windows Desktop (**Standalone Application**) mang phong cách **Dynamic Island** của iPhone lên hệ điều hành Windows 10 và Windows 11. 

Được thiết kế giao diện **Liquid Glass (Kính lỏng cao cấp)**, hiệu ứng chuyển động mượt mà tự nhiên, tích hợp Media Session thời gian thực, thông báo hệ thống và tối ưu hóa tài nguyên cực thấp dành riêng cho cả máy cấu hình yếu.

---

## ✨ Tính Năng Nổi Bật

### 1. Giao Diện Kính Lỏng Liquid Glass
- **Hiệu ứng khúc xạ mờ cao cấp:** Tích hợp Windows Composition / Acrylic & DWM Blur qua Win32 Native P/Invoke.
- **Specular Sheen & Rim Highlight:** Dải phản chiếu ánh sáng và viền kính sáng tinh xảo chuẩn phong cách Apple.
- **Soft Shadow đa tầng:** Đổ bóng mềm mại, không gây gánh nặng render GPU.
- **Tự động chuyển đổi Dark / Light / Hệ thống:** Tương thích hoàn hảo với chủ đề giao diện Windows.

### 2. Hai Chế Độ Linh Hoạt (Compact & Expanded)
- **Chế độ thu gọn (Compact Mode):** Nằm gọn gàng ở giữa mép trên màn hình. Hiển thị biểu tượng ứng dụng, tên bài hát/thông báo và sóng nhạc Equalizer 4 thanh động.
- **Chế độ mở rộng (Expanded Mode):** Khi nhấp chuột hoặc có sự kiện, đảo mở rộng mềm mại với gia tốc tự nhiên (Cubic Easing / Spring physics), nội dung mờ dần (Fade In / Fade Out) mượt mà.

### 3. Điều Khiển Nhạc Thời Gian Thực (Windows Media Session)
- Tích hợp chuẩn **Windows GSMTC** (`GlobalSystemMediaTransportControlsSessionManager`).
- Tự động bắt thông tin từ Spotify, Chrome, Edge, YouTube Music, iTunes, v.v.
- Hiển thị tên bài hát, nghệ sĩ, ảnh bìa Album (Album Artwork) sắc nét.
- Nút Play/Pause, Next, Previous tương tác trực tiếp với trình phát.
- Thanh tiến trình thời gian thực và Visualizer sóng âm thanh sống động.

### 4. Hệ Thống Thông Báo (Notification Queue)
- Nhận và hiển thị thông báo với biểu tượng, tiêu đề và chi tiết.
- Tự động mở rộng Dynamic Island khi có thông báo mới và tự đóng sau khoảng thời gian tùy chỉnh.

### 5. Giám Sát Phần Cứng Thông Minh (Low-Overhead System Monitor)
- Đo lường CPU siêu nhẹ qua `GetSystemTimes` của Windows NT Kernel (không dùng WMI hay PerformanceCounter tốn CPU).
- Theo dõi RAM qua `GlobalMemoryStatusEx` và thông tin Pin qua `GetSystemPowerStatus`.
- Chu kỳ cập nhật tối ưu, mức tiêu thụ CPU ở trạng thái chờ gần như **0%**.

### 6. Tối Ưu Máy Yếu (Low Performance Mode)
- **CPU Idle:** Gần **0%**.
- **RAM sử dụng:** Cố gắng dưới **100 MB**.
- Tự động phát hiện cấu hình máy yếu hoặc chế độ *Windows Reduce Motion* để giảm bớt hiệu ứng chuyển cảnh và hạ tần số vẽ.

### 7. Khay Hệ Thống & Cài Đặt (System Tray & Settings)
- Biểu tượng thu nhỏ dưới góc màn hình (System Tray).
- Menu chuột phải: Bật/Tắt hiển thị, Bật nhanh chế độ máy yếu, Mở cửa sổ Cài đặt, Thoát an toàn.
- Cửa sổ cài đặt giao diện đẹp mắt, lưu cấu hình định dạng JSON tại `%APPDATA%\DynamicIsland\settings.json`.
- Tùy chọn khởi động cùng Windows (Startup with Windows) an toàn thông qua Registry.

---

## 🚀 Hướng Dẫn Chạy & Cài Đặt

### Chạy Ngay Không Cần Cài Đặt (Khuyên Dùng)
Bản phát hành độc lập (Self-Contained) đã được đóng gói sẵn toàn bộ môi trường chạy tại thư mục `publish/`:

1. Vào thư mục `publish/`.
2. Chạy file `DynamicIslandApp.exe`.
3. Hoặc nhấp đúp vào file `run.bat` ở thư mục gốc ứng dụng để khởi động ngay.

---

## 🛠 Hướng Dẫn Tự Biên Dịch (Build from Source)

### Yêu cầu hệ thống:
- Hệ điều hành: Windows 10 (version 19041 trở lên) hoặc Windows 11.
- SDK: .NET 8 SDK (chạy lệnh `dotnet --version` để kiểm tra).

### Lệnh biên dịch:
```powershell
# 1. Chuyển vào thư mục dự án
cd DynamicIslandApp

# 2. Build bản Debug để chạy thử
dotnet build

# 3. Build bản Release
dotnet build -c Release

# 4. Đóng gói bản Self-Contained Single-File (chạy trên bất kỳ máy nào không cần cài .NET)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

---

## 📂 Cấu Trúc Dự Án

```
DynamicIslandApp/
├── App.xaml / App.xaml.cs          # Khởi động, Mutex kiểm tra đơn phiên bản, Global Exception
├── app.manifest                    # Cấu hình Windows 10/11 Per-Monitor V2 DPI Awareness
├── DynamicIslandApp.csproj         # Cấu hình .NET 8 WPF Win10 SDK
├── run.bat                         # Kịch bản khởi chạy nhanh
├── publish.bat                     # Kịch bản tự động đóng gói Single-File
│
├── Controls/
│   ├── WaveformVisualizer.xaml     # Equalizer 4 cột sóng âm thanh động
│   └── WaveformVisualizer.xaml.cs  # Logic nhịp sóng nhẹ nhàng, tắt khi Pause
│
├── Models/
│   ├── AppSettings.cs              # Model cấu hình JSON
│   ├── MediaInfo.cs                # Model dữ liệu phát nhạc GSMTC
│   ├── NotificationItem.cs         # Model thông báo
│   └── SystemStats.cs              # Model chỉ số CPU, RAM, Pin
│
├── Native/
│   ├── PInvoke.cs                  # Khai báo Win32 API: DWM, User32, Kernel32
│   └── WindowBlurHelper.cs         # Xử lý hiệu ứng nền kính Acrylic / DWM Blur
│
├── Services/
│   ├── SettingsManager.cs          # Đọc/ghi cấu hình JSON & phát hiện Theme Windows
│   ├── PerformanceManager.cs       # Đánh giá cấu hình phần cứng & kiểm soát hiệu ứng
│   ├── MediaSessionService.cs      # Lắng nghe Windows GSMTC đa phương tiện
│   ├── SystemMonitorService.cs     # Giám sát CPU/RAM/Pin chu kỳ thấp
│   ├── NotificationService.cs      # Quản lý hàng đợi và hiển thị thông báo
│   ├── StartupManager.cs           # Cài đặt tự khởi động cùng Windows
│   └── TrayIconManager.cs          # Biểu tượng và menu khay hệ thống
│
└── UI/
    ├── MainWindow.xaml             # Giao diện chính Dynamic Island (Liquid Glass)
    ├── MainWindow.xaml.cs          # Xử lý Animation mở rộng/thu gọn, phím tắt & tương tác
    ├── SettingsWindow.xaml         # Cửa sổ thiết lập cấu hình đầy đủ
    └── SettingsWindow.xaml.cs      # Logic lưu, reset cấu hình & kiểm thử thông báo
```

---

## 🎮 Thao Tác & Phím Tắt

- **Nhấp chuột trái vào đảo:** Mở rộng (Expand) hoặc Thu gọn (Collapse).
- **Nhấp đúp chuột trái:** Chuyển nhanh giữa chế độ nhỏ gọn và mở rộng.
- **Các nút điều khiển nhạc:** 
  - `⏮` : Bài hát trước
  - `▶ / ⏸` : Tạm dừng / Phát tiếp
  - `⏭` : Bài kế tiếp
- **Nút thu gọn `▲`:** Đóng đảo về lại thanh nhỏ phía trên.
- **Chuột phải vào icon khay hệ thống (System Tray):**
  - *Hiện / Ẩn Dynamic Island*
  - *Tối ưu máy yếu (Low Performance)*
  - *Cài đặt (Settings)...*
  - *Thoát (Exit)*

---

## 🛡 Kiểm Soát Lỗi & Độ Ổn Định

- Đảm bảo cơ chế **Mutex Single-Instance** (chỉ có duy nhất 1 phiên bản chạy, không bị nhân đôi icon khay hệ thống).
- Cơ chế bắt lỗi toàn cục `DispatcherUnhandledException` và `AppDomain.UnhandledException` đảm bảo không bị crash khi gặp lỗi bất ngờ từ API hệ thống hoặc driver âm thanh.
- Tự động định vị lại tọa độ giữa màn hình khi thay đổi độ phân giải màn hình hoặc cắm thêm màn hình phụ (`DisplaySettingsChanged`).

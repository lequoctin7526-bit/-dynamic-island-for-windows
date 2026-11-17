# Dynamic Island for Windows (Liquid Glass Edition)

[![Language](https://img.shields.io/badge/Language-Vietnamese-red.svg)](https://github.com/lequoctin7526-bit/-dynamic-island-for-windows)
[![Windhawk Mod](https://img.shields.io/badge/Windhawk-Mod-blue.svg)](https://windhawk.net/)
[![.NET 8 WPF](https://img.shields.io/badge/.NET-8.0%20WPF-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Dự án mang trải nghiệm **Dynamic Island** của iPhone lên hệ điều hành Windows 10 và Windows 11 với giao diện kính lỏng trong suốt **Liquid Glass**, animation siêu mượt, tối ưu cho máy cấu hình yếu và hỗ trợ **Tiếng Việt 100%**.

Dự án bao gồm 2 giải pháp hoàn chỉnh cho bạn lựa chọn:
1. 📱 **[Dynamic Island App](file:///c:/Users/Quoctin/Documents/winhard/DynamicIslandApp/README.md) (Khuyên dùng):** Ứng dụng Windows Desktop độc lập (Standalone Application) viết bằng C# .NET 8, WPF, Win32 P/Invoke & Windows GSMTC Media Session, chạy trực tiếp mà không cần Windhawk.
2. 🦅 **Windhawk Mod:** Bản Mod C++ nạp trực tiếp vào ứng dụng [Windhawk](https://windhawk.net/).

---

## 🌟 Giải Pháp 1: Ứng Dụng Độc Lập Windows Desktop (DynamicIslandApp)

Thư mục: `DynamicIslandApp/`

### Điểm nổi bật:
- **Giao diện Liquid Glass:** Acrylic / DWM Blur thời gian thực, dải bóng phản chiếu ánh sáng Specular Sheen và viền kính sáng Rim Highlight.
- **Điều khiển phát nhạc (GSMTC):** Tự động đồng bộ với Spotify, Chrome, Edge, YouTube Music, iTunes... Hiển thị tên bài hát, nghệ sĩ, ảnh Album và visualizer sóng nhạc động.
- **Hệ thống thông báo thông minh:** Tự động mở rộng khi có thông báo Windows và thu gọn sau thời gian định sẵn.
- **Giám sát phần cứng siêu nhẹ:** Đo lường CPU bằng Kernel System Times (không dùng WMI), RAM và Pin với CPU Idle gần 0%.
- **Khay hệ thống (System Tray):** Menu chuột phải, bật/tắt nhanh chế độ máy yếu, cửa sổ cài đặt JSON và khởi động cùng Windows.
- **Bản phát hành độc lập:** Đã đóng gói sẵn Single-File .exe tại `DynamicIslandApp/publish/DynamicIslandApp.exe`, chỉ cần mở là chạy.

👉 Xem chi tiết tại [DynamicIslandApp/README.md](file:///c:/Users/Quoctin/Documents/winhard/DynamicIslandApp/README.md).

---

## 🦅 Giải Pháp 2: Bản Mod Windhawk (dynamic-island-for-windows)

File mã nguồn: `dynamic-island-for-windows.wh.cpp`

### Hướng dẫn cài đặt vào Windhawk:
1. Mở ứng dụng **Windhawk** trên máy tính.
2. Tìm mod **Dynamic Island for Windows** và bấm **Chi tiết** (*Details*).
3. Chuyển sang tab **Mã nguồn** (*Source code*).
4. Nhấn `Ctrl + A` để chọn tất cả mã cũ, sau đó copy toàn bộ nội dung file [dynamic-island-for-windows.wh.cpp](file:///c:/Users/Quoctin/Documents/winhard/dynamic-island-for-windows.wh.cpp) dán đè vào.
5. Nhấn **Biên dịch & Lưu** (*Compile & Save*) ở góc phải trên. Bản mod sẽ tự nạp ngay lập tức!

---

## 🛠 Yêu Cầu Hệ Thống
- **Hệ điều hành:** Windows 10 (bản 19041 trở lên) hoặc Windows 11.
- **Hiệu ứng mờ:** Nên bật *Transparency effects* trong *Windows Settings > Personalization > Colors*.

---

## 📜 Bản Quyền & Tác Giả
- Phát triển và hoàn thiện bởi **Lê Quốc Tín**.
- Mã nguồn mở phát hành theo giấy phép **MIT License**.

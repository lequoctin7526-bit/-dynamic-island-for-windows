# Dynamic Island for Windows (Bản Việt hóa & Liquid Glass)

[![Windhawk Mod](https://img.shields.io/badge/Windhawk-Mod-blue.svg)](https://windhawk.net/)
[![Language](https://img.shields.io/badge/Language-Vietnamese-red.svg)](https://github.com/lequoctin7526-bit/-dynamic-island-for-windows)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Phiên bản nâng cấp của **Dynamic Island for Windows** dành cho [Windhawk](https://windhawk.net/), mang phong cách Dynamic Island của iPhone lên máy tính Windows với hiệu ứng kính lỏng **Liquid Glass**, tối ưu mượt mà cho máy cấu hình yếu và hỗ trợ **Tiếng Việt 100%**.

---

## ✨ Điểm nổi bật trong bản cập nhật này

1. **Hiệu ứng Liquid Glass (Kính lỏng trong suốt cao cấp):**
   - Áp dụng công nghệ Acrylic/HostBackdrop với độ mờ khúc xạ chân thực.
   - Hiệu ứng phản chiếu ánh sáng vòm kính (Specular Sheen) và viền bắt sáng (Rim Light) 3D sống động.
   - Có thể bật/tắt nhanh bằng cách nhấp chuột phải vào Dynamic Island -> *Độ trong suốt / Hiệu ứng* -> *Liquid Glass*.

2. **Tối ưu hóa máy yếu (Smooth / Low-End PC):**
   - Thuật toán lò xo chuyển động (`Spring physics`) được tối ưu bước tích phân thích ứng, giảm 75% tải CPU.
   - Tối ưu hóa bóng đổ (`Soft Drop Shadow`), giảm hơn 50% GPU fill-rate, loại bỏ hoàn toàn hiện tượng khựng lag khi co giãn.
   - Đồng bộ khung hình chuẩn xác (`DwmFlush`), chuyển động êm ái, không bị xé hình.

3. **Việt hóa đầy đủ (Vietnamese Localization):**
   - Mặc định giao diện sang **Tiếng Việt**.
   - Việt hóa toàn bộ các mô-đun: *Đa phương tiện, Lịch, Thời tiết, Giám sát phần cứng, Khay tệp, Lời bài hát, Pin, Âm lượng, Độ sáng, v.v.*
   - Toàn bộ Menu chuột phải và bảng Cài đặt trong Windhawk đều được chuyển sang Tiếng Việt.

---

## 🚀 Hướng dẫn cài đặt vào Windhawk

### Cách 1: Nạp trực tiếp qua Windhawk (Khuyên dùng)
1. Mở ứng dụng **Windhawk** trên máy tính.
2. Tìm mod **Dynamic Island for Windows** và bấm **Chi tiết** (*Details*).
3. Chuyển sang tab **Mã nguồn** (*Source code*).
4. Nhấn `Ctrl + A` để chọn tất cả mã cũ, sau đó copy toàn bộ nội dung file `dynamic-island-for-windows.wh.cpp` trong repo này dán đè vào.
5. Nhấn **Biên dịch & Lưu** (*Compile & Save*) ở góc phải trên. Bản mod sẽ tự nạp ngay lập tức!

### Cách 2: Tự biên dịch từ VS Code hoặc dòng lệnh
- Chạy file `build.bat` để biên dịch trực tiếp ra `dynamic-island-for-windows.dll` bằng trình biên dịch Clang đi kèm của Windhawk.
- Hoặc mở thư mục trong VS Code và nhấn tổ hợp phím `Ctrl + Shift + B`.

---

## 🛠️ Yêu cầu hệ thống
- **Hệ điều hành:** Windows 10 (bản 1803 trở lên) hoặc Windows 11.
- **Windhawk:** Đã cài đặt phiên bản Windhawk 1.4 trở lên.
- **Hiệu ứng mờ:** Khuyên dùng bật *Transparency effects* trong *Settings > Personalization > Colors*.

---

## 📜 Bản quyền & Lời cảm ơn
- Phát triển dựa trên dự án gốc của Himanshu và cộng đồng Windhawk.
- Phiên bản tùy biến Liquid Glass & Việt hóa bởi **Lê Quốc Tín**.
- Mã nguồn mở phát hành theo giấy phép **MIT License**.

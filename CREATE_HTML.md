# Frontend - Tạo index.html

## 📋 Bước 1: Tạo wwwroot folder

```bash
# Cách 1: PowerShell
New-Item -ItemType Directory -Force -Path "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot"

# Cách 2: Command Prompt  
mkdir "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot"

# Cách 3: Windows Explorer
# Tạo folder "wwwroot" trong SalesApi
```

## 📄 Bước 2: Copy HTML content

Tạo file: `D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\index.html`

Sao chép nội dung từ file: `frontend-index.html` (trong session này)

Hoặc download từ:
```
https://github.com/your-repo/SalesApi/wwwroot/index.html
```

## ✅ Bước 3: Xác nhận

Check xem file tồn tại:
```bash
ls "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\"
```

Output:
```
index.html
```

## 🚀 Bước 4: Chạy

```bash
cd D:\Tôn Bảo\DuAnMoi\SalesApi
dotnet run
```

Truy cập: `https://localhost:5050`

---

## 📝 Nội dung index.html

File HTML chứa:
- Login form
- Dashboard với thống kê
- Quản lý khách hàng
- Quản lý sản phẩm  
- Quản lý đơn hàng
- Responsive design
- Tailwind CSS styling
- React 18 CDN

**Kích thước:** ~50KB (compressed)
**Không cần build!** Chỉ copy file thôi.

---

## 💡 Mẹo

Nếu không muốn tạo file HTML, có thể:
1. Tạo React project riêng
2. Build và copy dist/* vào wwwroot
3. Hoặc dùng npm để package

Nhưng cách đơn giản nhất là copy HTML file này.

---

**Done! 🎉**

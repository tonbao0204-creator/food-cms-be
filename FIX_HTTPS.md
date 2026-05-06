# 🚀 FIX - Cách chạy SalesApi (Lỗi HTTPS được sửa)

## ⚡ Giải pháp nhanh (3 bước)

### Bước 1: Chạy script tạo frontend
```bash
"D:\Tôn Bảo\DuAnMoi\SalesApi\create-frontend.bat"
```

Hoặc tạo folder thủ công:
```bash
mkdir "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot"
```

### Bước 2: Setup Database
```bash
cd "D:\Tôn Bảo\DuAnMoi\SalesApi"
dotnet restore
dotnet ef database update
```

### Bước 3: Chạy Backend
```bash
dotnet run
```

---

## 🌐 Truy cập

✅ **HTTP (không HTTPS - không lỗi chứng chỉ)**
```
http://localhost:5050
```

**Username:** admin  
**Password:** admin123

---

## 📝 Những gì đã sửa

1. ✅ Thêm HTTP endpoint (port 5050)
2. ✅ Tạo script `create-frontend.bat` tự động tạo wwwroot + index.html
3. ✅ appsettings.json cập nhật

---

## 🔧 Nếu vẫn lỗi

### Kiểm tra SQL Server
```bash
# Mở Services (services.msc) kiểm tra SQL Server đang chạy
```

### Kiểm tra Port
```bash
# Mở PowerShell
netstat -ano | findstr :5050
```

### Xóa wwwroot cũ
```bash
rmdir /s "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot"
```

Rồi chạy lại `create-frontend.bat`

---

## ✅ Xác nhận thành công

Khi chạy `dotnet run`, bạn sẽ thấy:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5050
```

Sau đó truy cập: `http://localhost:5050`

---

**Thử ngay! 🎉**

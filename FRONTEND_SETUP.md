# Frontend HTML Setup Guide

Đây là hướng dẫn tạo thư mục `wwwroot` và thêm file `index.html`.

## Bước 1: Tạo wwwroot folder

Mở PowerShell/Command Prompt tại `D:\Tôn Bảo\DuAnMoi\SalesApi\` và chạy:

```bash
mkdir wwwroot
cd wwwroot
```

## Bước 2: Tạo index.html

Tạo file `D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\index.html`

Nội dung file có sẵn trong: `README_FRONTEND.md`

## Bước 3: Chạy backend

```bash
dotnet run
```

## Bước 4: Truy cập

Mở browser: `https://localhost:5050`

---

**Ghi chú:** File HTML được tạo bằng React CDN (không cần build), tất cả CSS inline, rất dễ deploy.

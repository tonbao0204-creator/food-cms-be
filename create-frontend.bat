@echo off
REM Create wwwroot and index.html

echo Creating wwwroot directory...
mkdir "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot" 2>nul

echo.
echo Creating index.html...

(
echo ^<!DOCTYPE html^>
echo ^<html lang="vi"^>
echo ^<head^>
echo ^    ^<meta charset="UTF-8"^>
echo ^    ^<meta name="viewport" content="width=device-width, initial-scale=1.0"^>
echo ^    ^<title^>SalesAPI - Quản Lý Bán Hàng^</title^>
echo ^    ^<script src="https://cdn.tailwindcss.com"^>^</script^>
echo ^    ^<script src="https://unpkg.com/react@18/umd/react.production.min.js"^>^</script^>
echo ^    ^<script src="https://unpkg.com/react-dom@18/umd/react-dom.production.min.js"^>^</script^>
echo ^    ^<style^>
echo body { font-family: 'Segoe UI', sans-serif; background: #f7fafc; }
echo .sidebar { width: 280px; background: linear-gradient(135deg, #667eea 0%%, #764ba2 100%%); min-height: 100vh; position: fixed; left: 0; top: 0; padding: 20px; color: white; z-index: 10; }
echo .main { margin-left: 280px; padding: 30px; }
echo .card { background: white; border-radius: 10px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.08); margin-bottom: 20px; }
echo .btn { padding: 10px 16px; border: none; border-radius: 6px; cursor: pointer; font-weight: 500; display: inline-flex; align-items: center; gap: 8px; }
echo .btn-primary { background: linear-gradient(135deg, #667eea 0%%, #764ba2 100%%); color: white; }
echo .table { width: 100%%; border-collapse: collapse; }
echo .table th { background: #edf2f7; padding: 12px; text-align: left; font-weight: 600; border-bottom: 2px solid #cbd5e0; }
echo .table td { padding: 12px; border-bottom: 1px solid #e2e8f0; }
echo .alert { padding: 12px 16px; border-radius: 6px; margin-bottom: 16px; }
echo .alert-success { background: #c6f6d5; color: #22543d; }
echo .alert-error { background: #fed7d7; color: #742a2a; }
echo ^</style^>
echo ^</head^>
echo ^<body^>
echo ^    ^<div id="root"^>^</div^>
echo ^    ^<script type="module"^>
echo const API = 'http://localhost:5050/api';
echo const showMsg = (msg, ok = true) =^> alert(msg);
echo const api = {
echo   async request(method, ep, data) {
echo     const token = localStorage.getItem('token');
echo     const headers = { 'Content-Type': 'application/json' };
echo     if (token) headers['Authorization'] = `Bearer ${token}`;
echo     try {
echo       const r = await fetch(`${API}${ep}`, { method, headers, body: data ? JSON.stringify(data) : null });
echo       const j = await r.json();
echo       if (!r.ok) throw new Error(j.message);
echo       return j.data;
echo     } catch (e) { showMsg(e.message, false); throw e; }
echo   },
echo   get: ep =^> api.request('GET', ep),
echo   post: (ep, d) =^> api.request('POST', ep, d)
echo };
echo 
echo function LoginPage({ onLogin }) {
echo   const [user, setUser] = React.useState('admin');
echo   const [pass, setPass] = React.useState('admin123');
echo   const handleLogin = async (e) =^> {
echo     e.preventDefault();
echo     try {
echo       const r = await fetch(`${API}/auth/login`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ username: user, password: pass }) });
echo       const d = await r.json();
echo       if (d.data?.token) { localStorage.setItem('token', d.data.token); localStorage.setItem('user', d.data.username); onLogin(); }
echo     } catch (e) { showMsg('Lỗi đăng nhập', false); }
echo   };
echo   return React.createElement('div', { style: { minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'linear-gradient(135deg, #667eea 0%%, #764ba2 100%%)' } }, React.createElement('div', { style: { background: 'white', padding: '40px', borderRadius: '10px', width: '400px' } }, React.createElement('h1', null, '📊 SalesAPI'), React.createElement('form', { onSubmit: handleLogin }, React.createElement('input', { value: user, onChange: e =^> setUser(e.target.value), placeholder: 'Username', style: { width: '100%%', padding: '10px', marginBottom: '10px', border: '1px solid #ccc', borderRadius: '6px' } }), React.createElement('input', { type: 'password', value: pass, onChange: e =^> setPass(e.target.value), placeholder: 'Password', style: { width: '100%%', padding: '10px', marginBottom: '10px', border: '1px solid #ccc', borderRadius: '6px' } }), React.createElement('button', { type: 'submit', style: { width: '100%%', padding: '10px', background: '#667eea', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' } }, '🔐 Đăng nhập'))));
echo }
echo 
echo function Dashboard() {
echo   const [customers, setCustomers] = React.useState([]);
echo   React.useEffect(() =^> { api.get('/customers').then(setCustomers).catch(() =^> {}); }, []);
echo   return React.createElement('div', null, React.createElement('h2', null, '📊 Dashboard'), React.createElement('p', null, 'Tổng khách hàng: ' + customers.length), React.createElement('div', { className: 'card' }, React.createElement('h3', null, '👥 Khách hàng'), React.createElement('table', { className: 'table' }, React.createElement('thead', null, React.createElement('tr', null, React.createElement('th', null, 'ID'), React.createElement('th', null, 'Tên'), React.createElement('th', null, 'SĐT'), React.createElement('th', null, 'Địa chỉ'))), React.createElement('tbody', null, customers.map(c =^> React.createElement('tr', { key: c.id }, React.createElement('td', null, c.id), React.createElement('td', null, c.fullName), React.createElement('td', null, c.phoneNumber), React.createElement('td', null, c.address)))))));
echo }
echo 
echo function App() {
echo   const [logged, setLogged] = React.useState(!!localStorage.getItem('token'));
echo   return logged ? React.createElement('div', null, React.createElement('div', { className: 'sidebar' }, React.createElement('h1', { style: { fontSize: '24px', marginBottom: '30px' } }, '📊 SalesAPI'), React.createElement('button', { onClick: () =^> { localStorage.clear(); setLogged(false); }, className: 'btn btn-primary' }, 'Đăng xuất')), React.createElement('div', { className: 'main' }, React.createElement(Dashboard, null))) : React.createElement(LoginPage, { onLogin: () =^> setLogged(true) });
echo }
echo 
echo const root = ReactDOM.createRoot(document.getElementById('root'));
echo root.render(React.createElement(App, null));
echo ^    ^</script^>
echo ^</body^>
echo ^</html^>
) > "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\index.html"

echo.
echo ✅ Hoàn thành!
echo.
echo Các file đã được tạo:
echo   - D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\
echo   - D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot\index.html
echo.
echo Bây giờ chạy:
echo   cd D:\Tôn Bảo\DuAnMoi\SalesApi
echo   dotnet run
echo.
echo Rồi truy cập: http://localhost:5050
echo.
pause

# Setup wwwroot and index.html
$path = "D:\Tôn Bảo\DuAnMoi\SalesApi\wwwroot"
New-Item -ItemType Directory -Force -Path $path | Out-Null

$html = @'
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SalesAPI</title>
    <script src="https://cdn.tailwindcss.com"></script>
    <script src="https://unpkg.com/react@18/umd/react.production.min.js"></script>
    <script src="https://unpkg.com/react-dom@18/umd/react-dom.production.min.js"></script>
</head>
<body>
    <div id="root"></div>
    <script type="module">
        const API = 'http://localhost:5050/api';
        
        async function fetchCustomers() {
            const token = localStorage.getItem('token');
            const headers = { 'Content-Type': 'application/json' };
            if (token) headers['Authorization'] = `Bearer ${token}`;
            
            try {
                const r = await fetch(`${API}/customers`, { headers });
                const d = await r.json();
                console.log('Customers:', d);
                return d.data || [];
            } catch (e) {
                console.error('Error:', e);
                return [];
            }
        }

        function LoginPage({ onLogin }) {
            const [user, setUser] = React.useState('admin');
            const [pass, setPass] = React.useState('admin123');
            const [loading, setLoading] = React.useState(false);

            const handleLogin = async (e) => {
                e.preventDefault();
                setLoading(true);
                try {
                    const r = await fetch(`${API}/auth/login`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ username: user, password: pass })
                    });
                    const d = await r.json();
                    if (d.data?.token) {
                        localStorage.setItem('token', d.data.token);
                        localStorage.setItem('username', d.data.username);
                        onLogin();
                    } else {
                        alert('Đăng nhập thất bại: ' + d.message);
                    }
                } catch (e) {
                    alert('Lỗi: ' + e.message);
                } finally {
                    setLoading(false);
                }
            };

            return React.createElement('div', 
                { style: { minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' } },
                React.createElement('div', 
                    { style: { background: 'white', padding: '40px', borderRadius: '10px', boxShadow: '0 10px 30px rgba(0,0,0,0.3)', width: '90%', maxWidth: '400px' } },
                    React.createElement('h1', { style: { textAlign: 'center', fontSize: '28px', marginBottom: '30px' } }, '📊 SalesAPI'),
                    React.createElement('form', { onSubmit: handleLogin },
                        React.createElement('div', { style: { marginBottom: '15px' } },
                            React.createElement('label', { style: { display: 'block', marginBottom: '5px', fontWeight: '600' } }, 'Username'),
                            React.createElement('input', { 
                                type: 'text',
                                value: user, 
                                onChange: e => setUser(e.target.value),
                                style: { width: '100%', padding: '10px', border: '1px solid #ccc', borderRadius: '6px', fontSize: '14px' } 
                            })
                        ),
                        React.createElement('div', { style: { marginBottom: '20px' } },
                            React.createElement('label', { style: { display: 'block', marginBottom: '5px', fontWeight: '600' } }, 'Password'),
                            React.createElement('input', { 
                                type: 'password',
                                value: pass, 
                                onChange: e => setPass(e.target.value),
                                style: { width: '100%', padding: '10px', border: '1px solid #ccc', borderRadius: '6px', fontSize: '14px' } 
                            })
                        ),
                        React.createElement('button', 
                            { 
                                type: 'submit', 
                                disabled: loading,
                                style: { 
                                    width: '100%', 
                                    padding: '10px', 
                                    background: loading ? '#999' : '#667eea', 
                                    color: 'white', 
                                    border: 'none', 
                                    borderRadius: '6px', 
                                    cursor: loading ? 'wait' : 'pointer', 
                                    fontWeight: 'bold', 
                                    fontSize: '16px'
                                } 
                            }, 
                            loading ? '🔄 Đang đăng nhập...' : '🔐 Đăng nhập'
                        )
                    ),
                    React.createElement('p', { style: { textAlign: 'center', marginTop: '20px', color: '#666', fontSize: '12px' } }, 'Tài khoản demo: admin / admin123')
                )
            );
        }

        function Dashboard({ user, onLogout }) {
            const [customers, setCustomers] = React.useState([]);
            const [loading, setLoading] = React.useState(true);

            React.useEffect(() => {
                fetchCustomers().then(data => {
                    setCustomers(data);
                    setLoading(false);
                });
            }, []);

            return React.createElement('div',
                { style: { display: 'flex' } },
                React.createElement('div', 
                    { style: { width: '280px', background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', color: 'white', padding: '20px', minHeight: '100vh', position: 'fixed', left: 0, top: 0 } },
                    React.createElement('h1', { style: { fontSize: '24px', marginBottom: '30px' } }, '📊 SalesAPI'),
                    React.createElement('nav', { style: { marginTop: '30px' } },
                        React.createElement('p', { style: { marginBottom: '20px' } }, '📈 Dashboard'),
                        React.createElement('p', { style: { marginBottom: '20px' } }, '👥 Khách hàng (' + customers.length + ')'),
                        React.createElement('p', { style: { marginBottom: '20px' } }, '🍔 Sản phẩm'),
                        React.createElement('p', { style: { marginBottom: '20px' } }, '📦 Đơn hàng')
                    ),
                    React.createElement('div', { style: { marginTop: '40px', paddingTop: '20px', borderTop: '1px solid rgba(255,255,255,0.2)' } },
                        React.createElement('p', { style: { marginBottom: '10px' } }, '👤 ' + user),
                        React.createElement('button', 
                            { 
                                onClick: onLogout,
                                style: { width: '100%', padding: '10px', background: 'rgba(255,255,255,0.2)', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }
                            }, 
                            'Đăng xuất'
                        )
                    )
                ),
                React.createElement('div', 
                    { style: { marginLeft: '280px', padding: '30px', width: 'calc(100% - 280px)' } },
                    React.createElement('h2', { style: { fontSize: '28px', marginBottom: '30px' } }, '📊 Dashboard'),
                    loading 
                        ? React.createElement('p', null, '⏳ Đang tải...')
                        : React.createElement('div',
                            { style: { background: 'white', padding: '20px', borderRadius: '10px', boxShadow: '0 2px 8px rgba(0,0,0,0.1)' } },
                            React.createElement('h3', null, '👥 Danh sách khách hàng (' + customers.length + ')'),
                            React.createElement('table',
                                { style: { width: '100%', borderCollapse: 'collapse', marginTop: '20px' } },
                                React.createElement('thead', null,
                                    React.createElement('tr', { style: { background: '#f0f0f0' } },
                                        React.createElement('th', { style: { padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' } }, 'ID'),
                                        React.createElement('th', { style: { padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' } }, 'Tên'),
                                        React.createElement('th', { style: { padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' } }, 'SĐT'),
                                        React.createElement('th', { style: { padding: '10px', textAlign: 'left', borderBottom: '2px solid #ddd' } }, 'Địa chỉ')
                                    )
                                ),
                                React.createElement('tbody', null,
                                    customers.map(c => React.createElement('tr', { key: c.id, style: { borderBottom: '1px solid #eee' } },
                                        React.createElement('td', { style: { padding: '10px' } }, c.id),
                                        React.createElement('td', { style: { padding: '10px' } }, c.fullName),
                                        React.createElement('td', { style: { padding: '10px' } }, c.phoneNumber || '-'),
                                        React.createElement('td', { style: { padding: '10px' } }, c.address || '-')
                                    ))
                                )
                            ),
                            customers.length === 0 && React.createElement('p', { style: { marginTop: '20px', color: '#999' } }, 'Không có khách hàng')
                        )
                )
            );
        }

        function App() {
            const [logged, setLogged] = React.useState(!!localStorage.getItem('token'));
            const [user, setUser] = React.useState(localStorage.getItem('username') || 'Admin');

            const handleLogin = () => {
                setUser(localStorage.getItem('username') || 'Admin');
                setLogged(true);
            };

            const handleLogout = () => {
                localStorage.clear();
                setLogged(false);
            };

            return logged 
                ? React.createElement(Dashboard, { user, onLogout: handleLogout })
                : React.createElement(LoginPage, { onLogin: handleLogin });
        }

        const root = ReactDOM.createRoot(document.getElementById('root'));
        root.render(React.createElement(App, null));
    </script>
</body>
</html>
'@

Set-Content -Path "$path\index.html" -Value $html -Encoding UTF8
Write-Host "✅ Tạo xong: $path\index.html"

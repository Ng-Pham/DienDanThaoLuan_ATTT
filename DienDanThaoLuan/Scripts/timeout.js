
document.addEventListener('DOMContentLoaded', function () {
    let idleTime = 0;
    const maxIdleTime = 10; // Đơn vị: phút

    // Tăng idleTime mỗi phút
    setInterval(function () {
        idleTime++;
        console.log('Idle minutes:', idleTime);

        if (idleTime < maxIdleTime) {
            fetch('/Account/KeepAlive', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => {
                if (response.ok) {
                    console.log('KeepAlive success');
                    // Không reset idleTime ở đây!
                } else {
                    console.log('KeepAlive failed');
                }
            })
            .catch(error => {
                console.log('KeepAlive error:', error);
            });
        }
    }, 60 * 1000); // 1 phút

    // Reset idleTime khi người dùng tương tác
    ['mousemove', 'keypress', 'click'].forEach(event => {
        document.addEventListener(event, () => {
            idleTime = 0;
            console.log('User active: idleTime reset to 0');
        });
    });
});

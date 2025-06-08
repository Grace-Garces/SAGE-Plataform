document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const errorMsg = document.getElementById('error-msg');

    loginForm.addEventListener('submit', async (event) => {
        event.preventDefault(); // Impede o envio padrão do formulário

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        // URL da sua API de login. Certifique-se de que a porta esteja correta.
        const apiUrl = 'https://localhost:7196/api/Auth/login'; // Ou http://localhost:5xxx se estiver usando HTTP

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });

            const data = await response.json();

            if (response.ok) {
                // Login bem-sucedido
                errorMsg.textContent = data.message;
                errorMsg.style.color = 'green';
                // Redirecionar para a página inicial, painel do usuário, etc.
                // Exemplo: window.location.href = 'dashboard.html';
                console.log('Login bem-sucedido:', data);
            } else {
                // Login falhou
                errorMsg.textContent = data.message || 'Erro desconhecido ao logar.';
                errorMsg.style.color = 'red';
                console.error('Erro de login:', data);
            }
        } catch (error) {
            console.error('Erro de rede ou na requisição:', error);
            errorMsg.textContent = 'Não foi possível conectar ao servidor. Tente novamente.';
            errorMsg.style.color = 'red';
        }
    });
});
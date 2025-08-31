const TodoList = document.querySelector('#TodoList');

// document.querySelector('#displayTodoList').addEventListener('click', listTodos);
document.querySelector('#login').addEventListener('click', login);
document.querySelector('#logout').addEventListener('click', logout);

const baseApiUrl = 'https://localhost:5001/api';

// xsrf token från cookie
function getXsrfToken() {
  return document.cookie
    .split('; ')
    .find((row) => row.startsWith('XSRF-TOKEN='))
    ?.split('=')[1];
}

// const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

async function listTodos() {
  console.log('Things to do!');
  const cookie = await cookieStore.get('.AspNetCore.Identity.Application');
  console.log('Cookie', cookie);

  const response = await fetch(`${baseApiUrl}/Todos`, {
    method: 'GET',
    mode: 'cors',
    credentials: 'include',
  });

  if (response.ok) {
    const result = await response.json();
    console.log(result);
    displayTodo(result.data);
  } else {
    function displayError() {
      alert('You are not authorized. Please log in.');
    }
    if (response.status === 401) displayError();
  }
}
async function login() {
  const username = document.getElementById('username').value;
  const password = document.getElementById('password').value;

   console.log("Username:", username);
  console.log("Password:", password);

  const response = await fetch('https://localhost:5001/api/accounts/login',{
    method: 'POST',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ UserName: username, Password: password }),
  });

  if (response.ok) {
    const result = await response.json();
    console.log(result.message);
    console.log('Backend response:', result);
    console.log(`User ${username} logged in with role: ${result.role}`);

    if (result.role.includes('Admin')) {
      window.location.href = '../src/pages/Admin/admin.html';
      return;
    } else {
      window.location.href = '../src/pages/TodoPage/todo.html';
      return;
    }
  } else {
    document.getElementById('welcomeMessage').textContent =
      alert(' ⚠️ Inloggning misslyckades!!!');
  }
}

function displayTodo(todos) {
  TodoList.innerHTML = '';

  for (let todo of todos) {
    const div = document.createElement('div');
    div.textContent = todo.title ?? todo.Title ?? JSON.stringify(todo);

    TodoList.appendChild(div);
  }
}

function displayError() {
  TodoList.innerHTML = '<h2 style="color:red;">UNAUTHORIZED</h2>';
}

async function logout() {
  console.log('Log out');

  const response = await fetch(`${baseApiUrl}/accounts/logout`, {
    method: 'POST',
    credentials: 'include',
    headers: { 'X-XSRF-TOKEN': getXsrfToken() },
  });
  if (response.ok) {
    window.location.reload();

    console.log(response);
    TodoList.innerHTML = '';
  }
}

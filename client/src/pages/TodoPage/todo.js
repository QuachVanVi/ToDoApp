const todoForm = document.getElementById('todo-form');
const todoInput = document.getElementById('todo-input');
const todoList = document.getElementById('todo-list');
const clearAll = document.getElementById('clear');

function getXsrfToken() {
  return document.cookie
    .split('; ')
    .find((row) => row.startsWith('XSRF-TOKEN='))
    ?.split('=')[1];
}

// const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
const apiUrl = 'https://localhost:5001/api/todos';

const initApp = async () => {
  try {
    const response = await fetch(apiUrl, {
      method: 'GET',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    const result = await response.json();

    if (result.success) {
      result.data.forEach((todo) => addTodoToDom(todo));
    }
  } catch (error) {
    console.error('Error loading todos:', error);
  }
};

const addTodoToDom = (todo) => {
  const item = document.createElement('li');
  item.dataset.id = todo.id; 

  // ✅ Checkbox for completed
  const checkbox = document.createElement('input');
  checkbox.type = 'checkbox';
  checkbox.checked = todo.isCompleted || false;

 checkbox.addEventListener('change', () => {
    todo.isCompleted = checkbox.checked;
    console.log(` "${todo.title}" completed?`, todo.isCompleted);
  });

  item.appendChild(checkbox);
  // Title
  const titleEl = document.createElement('strong');
  titleEl.textContent = todo.title;
  item.appendChild(titleEl);

  // Delete button
  const button = document.createElement('button');
  button.classList.add('btn-link', 'text-red');
  const icon = document.createElement('i');
  button.appendChild(icon);
  button.addEventListener('click', async () => {
    try {
      const response = await fetch(
        `https://localhost:5001/api/todos/${todo.id}`,
        {
          method: 'DELETE',
          credentials: 'include',
          headers: { 'X-XSRF-TOKEN': getXsrfToken() },
        }
      );
      if (!response.ok) throw new Error('Failed to delete todo');

      item.remove();
    } catch (err) {
      console.error(err);
    }
  });
  item.appendChild(button);

  todoList.appendChild(item);
};

const handleAddTodo = async (e) => {
  e.preventDefault();

  const title = todoInput.value.trim();

  // Matchar backend-validering: minst 2 tecken, max 100
  if (title.length < 2) {
    alert('Titeln måste vara minst 2 tecken.');
    return;
  }
  if (title.length > 100) {
    alert('Titeln får inte vara längre än 100 tecken.');
    return;
  }

  const todo = { title };

  try {
    const response = await fetch('https://localhost:5001/api/todos', {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
        'X-XSRF-TOKEN': getXsrfToken(),
      },
      body: JSON.stringify(todo),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(
        `HTTP error! status: ${response.status}, message: ${errorText}`
      );
    }

    const result = await response.json();
    addTodoToDom(result.data);
    todoInput.value = '';
  } catch (error) {
    console.error('Error adding todo:', error);
    alert('Kunde inte lägga till todo. Kolla konsolen för mer info.');
  }
};


const handleClearList = async () => {
  try {
    const response = await fetch(`${apiUrl}/clearAll`, { method: 'DELETE',headers: {
        'Content-Type': 'application/json',
        'X-XSRF-TOKEN': getXsrfToken()
    },});
    const result = await response.json().catch(() => ({}));

    if (response.ok && result.success) {
      while (todoList.firstChild) {
        todoList.removeChild(todoList.firstChild);
      }
      console.log('All todos cleared successfully.');
    } else if (response.status === 404) {
      console.log('No todos to clear.');
    } else {
      console.error('Failed to clear todos:', result.error || 'Unknown error');
    }
  } catch (error) {
    console.error('Error clearing todos:', error);
  }
};

document.addEventListener('DOMContentLoaded', initApp);
todoForm.addEventListener('submit', handleAddTodo);
clearAll.addEventListener('click', handleClearList);

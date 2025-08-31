export const addToStorage = (todo) => {
  const todos = getFromStorage();
  todos.push(todo);
  sessionStorage.setItem('todos', JSON.stringify(todos));
};

export const getFromStorage = () => {
  let items;

  if (sessionStorage.getItem('todos') === null) {
    items = [];
  } else {
    items = JSON.parse(sessionStorage.getItem('todos'));
  }

  return items;
};

export const removeFromStorage = (todo) => {
  let todos = getFromStorage();

  todos = todos.filter((item) => item !== todo);
  sessionStorage.setItem('todos', JSON.stringify(todos));
};

export const clearStorage = (key) => {
  sessionStorage.removeItem(key);
};
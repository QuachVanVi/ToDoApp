import { config } from './config.js';

export const post = async (endpoint, data) => {
  const url = `${config.apiUrl}/${endpoint}`;

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });

    if (response.ok) {
      return await response.json();
    }
  } catch (error) {
    console.log(error);
  }
};
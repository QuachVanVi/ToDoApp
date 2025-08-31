



const post = async (endpoint, data) => {
    const baseApiUrl = 'https://localhost:5001/api';
    const url = `${baseApiUrl}/${endpoint}`;

    function getXsrfToken() {
    return document.cookie
        .split('; ')
        .find(row => row.startsWith('XSRF-TOKEN='))
        ?.split('=')[1];
}

    //   console.log('XSRF token:', xsrfToken);

    try {
        const response = await fetch(url, {
            method: 'POST',
            mode: 'cors',
            credentials: 'include',
            headers: { 
                'Content-Type': 'application/json', 'X-XSRF-TOKEN': getXsrfToken()
            },
            body: JSON.stringify(data),
        });

        console.log('Response status:', response.status, response.statusText);
        
        const contentType = response.headers.get('content-type');
        
        if (contentType && contentType.includes('application/json')) {
            const responseData = await response.json();
            console.log('Response body (JSON):', responseData);
            return responseData;
        } else {
            return { success: response.ok };
        }
    } catch (error) {
        console.error('Request failed:', error);
        throw error;
    }
};

const form = document.querySelector('#CreateAccountForm');

const handleCreateAcc = async (e) => {
    e.preventDefault();
    
    const userData = {
        FirstName: document.querySelector('#firstName').value,
        LastName: document.querySelector('#lastName').value,
        Email: document.querySelector('#email').value,
        Password: document.querySelector('#password').value,
        Role: document.querySelector('#role').value
    };

    console.log('Submitting data:', userData);
    const submitButton = form.querySelector('button[type="submit"]');
    const originalText = submitButton.textContent;
    submitButton.disabled = true;
    submitButton.textContent = 'Creating Account...';

    try {
        const result = await post('accounts/register', userData);
        
        if (result && result.success !== false) {
            console.log('Account created successfully');
            alert('Account created successfully!');
            form.reset();
            
          window.location.href = '/client/src/index.html';
            
        } else {
            console.error('Account creation failed:', result);
            alert('Account creation failed. Please try again.');
        }
    } catch (error) {
        console.error('Error creating account:', error);
        alert(`Error: ${error.message}`);
    } finally {
        
        submitButton.disabled = false;
        submitButton.textContent = originalText;
    }
};

form.addEventListener('submit', handleCreateAcc);
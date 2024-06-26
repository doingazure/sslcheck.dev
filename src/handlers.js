console.log('➡ JavaScript loaded'); // Indicates successful loading

document.querySelector('button').addEventListener('click', async () => {
    const domain = document.getElementById('domain').value;
    const resultContainer = document.getElementById('SslCheckResults');

    try {
        let json = await handleSslCheck(domain, resultContainer);
        console.log('🔥 handleSslCheck resolved with:', json); 
    } catch (error) {
        console.error('🔥 handleSslCheck rejected:', error);
        // Handle the error here, potentially display a user-friendly message
        alert(`Danger Will Robinson! Error - 🔥 handleSslCheck rejected with this response: ${error}`);
    }
});

async function handleSslCheck(domain, resultElement) {
    console.log(`Button clicked for domain = "${domain}"`);

    const api = `/api/SslCheck?domain=${domain}`; // Cleaner API endpoint
    // const api = `/SslCheck?domain=${domain}`; // Cleaner API endpoint
    console.log(`ATTEMPTING SslCheck API CALL: ${api}`);

    try {
        const response = await fetch(api, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
            } // Simplified headers
        });

        if (!response.ok) {
            console.error(`handleSslCheck called ${api} and about to throw because (!response.ok) with status: ${response.status}`);
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const json = await response.json();
        console.log(`WRITING RESULTING JSON into ${resultElement.id}:`, json);
        resultElement.innerText = JSON.stringify(json, null, 2); // Pretty-print JSON
        return json;
    } catch (error) {
        const errmsg = `Processing domain "${domain}" failed: ${error}`;
        console.error(errmsg);
        resultElement.innerText = errmsg; // Display the error message to the user
        throw error; // Rethrow the error for the calling function to handle
    }
}

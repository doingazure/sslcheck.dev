console.log('➡ javascript loaded');

document.querySelector('button').addEventListener('click', async () => {
   const domain = document.getElementById('domain').value;
   const resultContainer = document.getElementById('SslCheckResults');
   let json = await handleSslCheck(domain, resultContainer);
   // handle the resolved promise here
   console.log(`🔥 handleSslCheck resolved with ${json}`);
   console.log('🔥 handleSslCheck resolved');
});

//  function that can be called from HTML when the user clicks on the button
async function handleSslCheck(domain, resultElement) {
   console.log(`Button clicked for domain = "${domain}"`);

   // const api = `http://localhost:7071/api/SslCheck?domain=${domain}`;
   const api = `/api/SslCheck?domain=${domain}`;
   // const api = `/SslCheck?domain=${domain}`;

   console.log(`ATTEMPTING SslCheck API CALL THUSLY: ${api}`);
   // var json = (await fetch(api)).statusText; // HELPFUL

   fetch(api, {
      method: 'GET',
      headers: {
         'Accept': 'application/json',
         'Content-Type': 'application/json'
      }
   }).then(response => {
         if (!response.ok) {
            console.log(`HTTP error! Status: ${response.status}`);
            throw new Error(`HTTP error! Status: ${response.status}`);
         }
         return response.json();
      })
      .then(json => {
         console.log(`WRITING RESULTING JSON ${json} into ${resultElement.id}`);
         resultElement.innerText = JSON.stringify(json);
         return JSON.stringify(json);
      })
      .catch((error) => {
         let errmsg = `Processing domain "${domain}" failed with error: ${error}`;
         console.error(errmsg);
         resultElement.innerText = JSON.stringify(errmsg);
         return errmsg;
      });
}


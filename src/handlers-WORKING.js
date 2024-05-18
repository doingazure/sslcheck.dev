console.log('➡ javascript loaded');

document.querySelector('button').addEventListener('click', async () => {
   const domain = document.getElementById('domain').value;
   const resultContainer = document.getElementById('SslCheckResults');
   const json = await handleSslCheck(domain, resultContainer);
   console.log(`🔥 handleSslCheck resolved with ${json}`);
   console.log('🔥 handleSslCheck resolved');
});

async function handleSslCheck(domain, resultElement) {
   console.log(`Button clicked for domain = "${domain}"`);

   const api = `/api/SslCheck?domain=${domain}`;

   console.log(`ATTEMPTING SslCheck API CALL THUSLY: ${api}`);

   try {
      const response = await fetch(api, {
         method: 'GET',
         headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
         }
      });

      if (!response.ok) {
         throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const json = await response.json();
      console.log(`WRITING RESULTING JSON ${json} into ${resultElement.id}`);
      resultElement.textContent = JSON.stringify(json);
      return JSON.stringify(json);
   } catch (error) {
      const errmsg = `Processing domain "${domain}" failed with error: ${error}`;
      console.error(errmsg);
      resultElement.textContent = JSON.stringify(errmsg);
      return errmsg;
   }
}

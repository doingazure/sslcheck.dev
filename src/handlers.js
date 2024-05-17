//  function that can be called from HTML when the user clicks on the button
async function handleCheckSsl(domain) {
   console.log('Button clicked ' + domain);
   alert('Button clicked ' + domain);

   const api = `http://localhost:7071/api/SslCheck?domain=${domain}`;

   try {
      var json = await fetch(api);
      return json;
   } catch (error) {
      console.error(error);
      return error;
   }

}


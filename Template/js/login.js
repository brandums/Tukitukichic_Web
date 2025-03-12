const apiUrl = apiUrl3

const countries = countriesList;


document.addEventListener('DOMContentLoaded', () => {
    const loginOption = document.getElementById('login-option');
    const signupOption = document.getElementById('signup-option');
    const loginForm = document.getElementById('login-form');
    const signupForm = document.getElementById('signup-form');
    
    loginOption.addEventListener('click', () => {
        loginForm.classList.remove('hidden');
        signupForm.classList.add('hidden');
    });
    
    signupOption.addEventListener('click', () => {
        loginForm.classList.add('hidden');
        signupForm.classList.remove('hidden');
    });
    
    const countrySelect = document.getElementById('country');
    countries.forEach(country => {
        const option = document.createElement('option');
        option.value = country.dial_code;
        option.textContent = country.code + " - " + country.name;
        countrySelect.appendChild(option);
    });

    const countryCodeInput = document.getElementById('countryCode');
    countrySelect.addEventListener('change', function() {
        const selectedDialCode = this.value;
        const selectedCountry = countries.find(country => country.dial_code === selectedDialCode);

        if (selectedCountry) {
            countryCodeInput.value = selectedDialCode;

            if (selectedCountry.code === 'VE' || selectedCountry.code === 'HT' || selectedCountry.code === 'CM') {
                alert('Sorry, at the moment we do not have service for your country.');
                window.location.href = 'index.html';
            }
        } else {
            countryCodeInput.value = ''; 
        }
    });
});



// metodo para autenticar un usuario
document.addEventListener('DOMContentLoaded', function() {
    var loginForm = document.getElementById('login-form');

    loginForm.addEventListener('submit', function(event) {
        event.preventDefault();

        var user = document.getElementById('userId').value;
        var password = document.getElementById('password').value;

        var api = apiUrl + 'User/' + encodeURIComponent(user) + '/' + encodeURIComponent(password);

        fetch(api, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(function(response) {
            if (response.ok) {
                response.json().then(function(data) {
                    var userString = JSON.stringify(data.user);
                    document.cookie = "user=" + encodeURIComponent(userString) + "; expires=Thu, 18 Dec 2025 12:00:00 UTC; path=/";
        
                    let urlGuardada = localStorage.getItem("currentURL");
                    if(urlGuardada)
                    {
                        localStorage.removeItem("currentURL");
                        window.location.href = urlGuardada;
                    }
                    else{
                        window.location.href = 'index.html';
                    }                    
                });
            } else {
                alert('Incorrect username or password');
            }
        })
        .catch(function(error) {
            console.error('Error en la solicitud:', error);
            alert('Error in the request. Please try again later.');
        });
    });
});


// metodo para registrar un usuario
document.addEventListener('DOMContentLoaded', function() {
  var signupForm = document.getElementById('signup-form');

  signupForm.addEventListener('submit', function(event) {
      event.preventDefault();

      var name = document.getElementById('name').value;
      var email = document.getElementById('email').value;
      var city = document.getElementById('city').value;
      var countrySelect = document.getElementById('country');
      var selectedDialCode = countrySelect.value;
      var password = document.getElementById('passwordSign').value;
      var address = document.getElementById('address').value;
      var postcode = document.getElementById('postcode').value;
      var countryCode = document.getElementById('countryCode').value;
      var phoneNumber = document.getElementById('phoneNumber').value;

      var fullPhoneNumber = countryCode + phoneNumber;

      var selectedCountry = countries.find(country => country.dial_code === selectedDialCode);
      var countryName = selectedCountry ? selectedCountry.name : '';

      var user = {
          id : 0,
          AccountNumber: "",
          typeAccount : "Permanente",
          name: name,
          ci: countryName + "," + postcode,
          city: city,
          password: password,
          email: email,
          phone: fullPhoneNumber,
          gender: address,
      };

      var jsonUser = JSON.stringify(user);

      fetch(apiUrl + 'User', {
          method: 'POST',
          headers: {
              'Content-Type': 'application/json'
          },
          body: jsonUser
      })
      .then(function(response) {
          if (response.ok) {
              document.getElementById('login-option').click();
              alert("Account successfully created. Now you can log in.");
          } else {
              return response.text().then(function(errorMessage) {
                  throw new Error(errorMessage);
              });
          }
      })
      .catch(function(error) {
          console.error('Error creating the account:', error);
          alert('Error creating the account: ' + error.message);
      });
  });
});

// // metodo para seleccionar la opcion de "Email" o "ci"
// function toggleFields() {
//     var switchElement = document.getElementById("switch");
//     var ciInput = document.getElementById("divCI");
//     var emailInput = document.getElementById("divEmail");
    
//     if (switchElement.checked) {
//       ciInput.style.display = "none";
//       emailInput.style.display = "block";
//     } else {
//       ciInput.style.display = "block";
//       emailInput.style.display = "none";
//     }
//   }

function handleCredentialResponse(response) {
    const id_token = response.credential;
    console.log("ID Token:", id_token);

    const payload = JSON.parse(atob(id_token.split('.')[1]));
    const userName = `${payload.given_name} ${payload.family_name}`;
    const email = payload.email;

    SignUp(userName, email);
}

window.onload = function () {
    document.documentElement.lang = 'en';
    document.documentElement.setAttribute('lang', 'en');

    google.accounts.id.initialize({
        client_id: '437926497565-ap2vqgvalqs302saa4b57r1h7flu9mgk.apps.googleusercontent.com',
        callback: handleCredentialResponse,
        locale: 'en'
    });

    google.accounts.id.renderButton(
        document.getElementById('g_id_signin'),
        { theme: 'outline', size: 'large', text: 'sign_in_with', locale: 'en' }
    );
};

function SignUp(userName, email) {
    console.log("Registrando usuario...");
    console.log("Nombre de usuario:", userName);
    console.log("Correo electrónico:", email);

    fetch(apiUrl + `User/signup/${encodeURIComponent(userName)}/${encodeURIComponent(email)}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        if (response.ok) {
            return response.json();
        } else {
            throw new Error("Error en la solicitud");
        }
    })
    .then(data => {
        console.log("Respuesta del servidor:", data);

        const userString = JSON.stringify(data);
        document.cookie = "user=" + encodeURIComponent(userString) + "; expires=Thu, 18 Dec 2025 12:00:00 UTC; path=/";

        let urlGuardada = localStorage.getItem("currentURL");
        if (urlGuardada) {
            localStorage.removeItem("currentURL");
            window.location.href = urlGuardada;
        } else {
            window.location.href = '/index.html';
        }
    })
    .catch(error => {
        console.error("Error al registrar el usuario:", error);
    });
}
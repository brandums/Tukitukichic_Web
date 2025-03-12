const apiUrl = apiUrl3;
const countries = countriesList;
let previousShippingCost = 0;
let userLog;
let discount = 0;
let basePrice = 0;

document.addEventListener('DOMContentLoaded', async function () {
    const shippingPriceElement = document.getElementById('shippingPrice');
    const countrySelect = document.getElementById('country');
    const address = document.getElementById('address');
    const finalPriceElement = document.querySelector(".finalPrice");
    
    countries.forEach(country => {
        const option = document.createElement('option');
        option.value = country.name;
        option.textContent = country.code + " - " + country.name;
        option.dataset.code = country.code;
        countrySelect.appendChild(option);
    });

    var userCookie = getCookie('user');
    if (userCookie) {
        userLog = JSON.parse(decodeURIComponent(userCookie));
        address.value = userLog.gender;

        const countryFromCI = userLog.ci.split(',')[0].trim();
        const matchingCountry = countries.find(country => country.name.toLowerCase() === countryFromCI.toLowerCase());
        
        if (matchingCountry) {
            for (let i = 0; i < countrySelect.options.length; i++) {
                if (countrySelect.options[i].value === matchingCountry.name) {
                    countrySelect.selectedIndex = i;
                    countrySelect.dispatchEvent(new Event('change'));
                    break;
                }
            }
        }

        await obtenerDescuento(userLog.id);
        actualizarCostoEnvio();
    } else {
        discount = 0;
        actualizarCostoEnvio();
    }

    countrySelect.addEventListener('change', function () {
        const selectedOption = countrySelect.options[countrySelect.selectedIndex];
        const selectedCountryCode = selectedOption.dataset.code;
    
        if (['VE', 'HT', 'CM'].includes(selectedCountryCode)) {
            alert('Sorry, at the moment we do not have service for your country.');
            window.location.href = 'index.html';
            return;
        }
    
        let newShippingCost = 0;
        if (selectedCountryCode === 'US') {
            newShippingCost = 0;
        } else if (['GB', 'ES', 'FR', 'MX', 'CA'].includes(selectedCountryCode)) {
            newShippingCost = 8;
        } else {
            newShippingCost = 12;
        }
        
        shippingPriceElement.textContent = `${newShippingCost} $`;
        recalcularPrecioConShipping(newShippingCost);
        previousShippingCost = newShippingCost;
    });
});

function actualizarCostoEnvio() {
    const countrySelect = document.getElementById('country');
    const shippingPriceElement = document.getElementById('shippingPrice');
    const countryCode = countrySelect.value;

    let newShippingCost = 0;
    if (countryCode === 'US') {
        newShippingCost = 0;
    } else if (['GB', 'ES', 'FR', 'MX', 'CA'].includes(countryCode)) {
        newShippingCost = 8;
    } else {
        newShippingCost = 12;
    }

    shippingPriceElement.textContent = `${newShippingCost} $`;
    recalcularPrecioConShipping(newShippingCost);
}

async function obtenerDescuento(userId) {
    try {
        const response = await fetch(`${apiUrl}User/getDiscount/${userId}`);
        if (!response.ok) {
            throw new Error("Error al obtener el descuento.");
        }
        discount = parseFloat(await response.text());
        
        const discountText = document.getElementById('discount');
        discountText.textContent = discount + " %";

        recalcularPrecioConShipping(previousShippingCost);
    } catch (error) {
        console.error("Error:", error);
    }
}

function recalcularPrecioConShipping(newShippingCost) {
    const finalPriceElement = document.querySelector(".finalPrice");

    basePrice = parseFloat(finalPriceElement.dataset.basePrice);
    let totalPrice = basePrice + newShippingCost;
    totalPrice = aplicarDescuento(totalPrice);

    finalPriceElement.textContent = `${totalPrice.toFixed(2)} $`;
}

function aplicarDescuento(total) {
    return total - (total * discount / 100);
}

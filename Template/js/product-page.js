const apiUrl = apiUrl3;
let code = "";

document.addEventListener('DOMContentLoaded', function() {
    const urlParams = new URLSearchParams(window.location.search);
    code = urlParams.get('code');

    if (code) {
        loadProducts(code);
        loadRating();
        loadAllComments();
    }
});

function loadProducts(code){
    fetch(apiUrl + `PrincipalStruct/getProduct/${code}`)
    .then(response => response.json())
    .then(data => {
        this.showProduct(data, 0);
    })
    .catch(error => console.error('Error fetching data:', error));
}

function getVentasElement(productCode) {
    const url = `${apiUrl}PrincipalStruct/getNumberOfSales/${productCode}`;

    return fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Error al obtener el número de ventas: ${response.statusText}`);
            }
            return response.json();
        })
        .catch(error => {
            console.error('Error al obtener el número de ventas:', error);
            throw error;
        });
}



function showProduct(data, index)
{
    document.querySelector('.detial-holder h2').textContent = data.nombre[index];
    document.querySelector('.total-price').textContent = data.precio[index] + "$.";
    document.getElementById('descripcion').src = data.descripcion[index];
    document.getElementById('breveDescripcion').textContent = data.breveDescripcion[index];
    document.querySelector('.product-code').textContent = data.codigo[index];
    document.querySelector('.size').innerHTML = data.extra2[index];
    getVentasElement(data.codigo[index])
    .then(ventas => {
        document.getElementById('ventas').textContent = ventas;
    })
    .catch(error => {
        console.error('Error en la solicitud:', error);
    });
    iconFavorite = document.querySelector('.icon-favorite');
    if(data.images[index][13] !== null && data.images[index][13] !== "" && data.images[index][5] !== null && data.images[index][5] !== ""){
        document.getElementById('img1').src = data.images[index][13];
        document.getElementById('image1').src = data.images[index][5];
    }
    if(data.images[index][14] !== null && data.images[index][14] !== "" && data.images[index][6] !== null && data.images[index][6] !== ""){
        document.getElementById('img2').src = data.images[index][14];
        document.getElementById('image2').src = data.images[index][6];
    }
    if(data.images[index][15] !== null && data.images[index][15] !== "" && data.images[index][7] !== null && data.images[index][7] !== ""){
        document.getElementById('img3').src = data.images[index][15];
        document.getElementById('image3').src = data.images[index][7];
    }
    if(data.images[index][16] !== null && data.images[index][16] !== "" && data.images[index][8] !== null && data.images[index][8] !== ""){
        document.getElementById('img4').src = data.images[index][16];
        document.getElementById('image4').src = data.images[index][8];
    }
    if(data.images[index][17] !== null && data.images[index][17] !== "" && data.images[index][9] !== null && data.images[index][9] !== ""){
        document.getElementById('img5').src = data.images[index][17];
        document.getElementById('image5').style.display = 'block';
        document.getElementById('image5').src = data.images[index][9];
    }
    if(data.images[index][18] !== null && data.images[index][18] !== "" && data.images[index][10] !== null && data.images[index][10] !== ""){
        document.getElementById('img6').src = data.images[index][18];
        document.getElementById('image6').style.display = 'block';
        document.getElementById('image6').src = data.images[index][10];
    }
    if(data.images[index][19] !== null && data.images[index][19] !== "" && data.images[index][11] !== null && data.images[index][11] !== ""){
        document.getElementById('img7').src = data.images[index][19];
        document.getElementById('image7').style.display = 'block';
        document.getElementById('image7').src = data.images[index][11];
    }
    if(data.images[index][20] !== null && data.images[index][20] !== "" && data.images[index][12] !== null && data.images[index][12] !== ""){
        document.getElementById('img8').src = data.images[index][20];
        document.getElementById('image8').style.display = 'block';
        document.getElementById('image8').src = data.images[index][12];
    }

    fillSizeList(data.talla[index]);
    fillColorOptions(data.color[index]);

    addClickEventToImages(data.color[index])

    if (iconFavorite) {
        iconFavorite.dataset.code = data.codigo[index];

        getProductIndex(data.codigo[index]).then(result => {
            iconFavorite.dataset.favorite = result;
            iconFavorite.dataset.likes = 0;
            
            if (iconFavorite.dataset.favorite === '1') {
                iconFavorite.classList.add('favorite');
            } else {
                iconFavorite.classList.remove('favorite');
            }
        });
    }
    
    const addToCartBtn = document.querySelector('.btn-primary');
    addToCartBtn.dataset.code = data.codigo[index];

    var userCookie = getCookie('user');
    var user;
    if (userCookie) {
        user = JSON.parse(decodeURIComponent(userCookie));
    }

    if(user)
    {
        alreadyInCart();
        buttonAddProductToCart(user);
    }
    else
    {
        addToCartBtn.addEventListener('click', function(event) {
            event.preventDefault();
            localStorage.setItem("currentURL", window.location.href);
            $('#modalLogin').modal('show');
        })
    }



    var PreciElement = document.getElementById("precio1");
    var DescElement = document.getElementById("descuento1");
    const DescPrecio = data.extra7[index];
    if (data.extra8[index] === "1") {
        PreciElement.style.textDecoration = "line-through";
        DescElement.textContent = DescPrecio + "$.";

        document.getElementById("offerEndsText").style.display = "inline";
        document.getElementById("countdown").style.display = "inline";

        if(data.tiempoOferta[index] != "0"){
            startCountdown(data.tiempoOferta[index]);
        }        
    } else {
        PreciElement.style.textDecoration = "none";
        DescElement.textContent = "";
        document.getElementById("offerEndsText").style.display = "none";
        document.getElementById("countdown").style.display = "none";
    }
}

function updateCountdown(endDate) {
    const countdownElement = document.getElementById("countdown");

    const interval = setInterval(function () {
        fetch(`${apiUrl}PrincipalStruct/server-time`)
            .then(response => response.text())
            .then(serverTime => {
                const serverDate = new Date(serverTime);
                const now = serverDate.getTime();
                const distance = endDate - now;

                if (distance <= 0) {
                    clearInterval(interval);
                    countdownElement.textContent = "Offer ended";
                } else {
                    const days = Math.floor(distance / (1000 * 60 * 60 * 24));

                    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
                    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

                    countdownElement.textContent = `${days}d ${hours}h ${minutes}m ${seconds}s`;
                }
            })
            .catch(error => {
                console.error('Error fetching server time:', error);
            });
    }, 1000);
}



function startCountdown(time) {
    let offerEndDate;

    if (typeof time === "string") {
        offerEndDate = new Date(time.replace(" ", "T"));
    } else if (time instanceof Date) {
        offerEndDate = time;
    } else {
        console.error("Invalid time format:", time);
        return;
    }

    updateCountdown(offerEndDate);
}

function fillSizeList(sizes){
    addToCartBtn = document.querySelector('.btn-primary');
    const sizeList = document.querySelector('.size-list');
    sizeList.innerHTML = "";

    sizes.forEach((size, index) => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        a.textContent = size;
        li.appendChild(a);
        sizeList.appendChild(li);

        if (index === 0) {
            li.classList.add('active');
            addToCartBtn.dataset.talla = size;
        }

        li.addEventListener('click', function() {
            sizeList.querySelectorAll('li').forEach(item => {
                item.classList.remove('active');
            });
            li.classList.add('active');
            addToCartBtn.dataset.talla = size;
        });
    });
}

function fillColorOptions(colors) {
    const select = document.getElementById('clr');
    select.innerHTML = "";

    colors.forEach((color, i) => {
        const option = document.createElement('option');
        option.textContent = color;
        option.value = i;
        option.dataset.color = color;
        select.appendChild(option);
    });

    if (colors.length > 0) {
        select.dataset.color = colors[0]; 
    }

    select.addEventListener('change', function() {
        const selectedIndex = this.value;
        select.dataset.color = colors[selectedIndex];

        simulateClickOnImage(selectedIndex);
        alreadyInCart();
    });
}
function simulateClickOnImage(colorIndex) {
    const imgId = `image${parseInt(colorIndex) + 1}`;
    const imgElement = document.getElementById(imgId);
    
    if (imgElement) {
        imgElement.click();
    }
}
function addClickEventToImages(colors) {
    colors.forEach((color, i) => {
        const imgId = `image${i + 1}`;
        const imgElement = document.getElementById(imgId);

        if (imgElement) {
            imgElement.addEventListener('click', function() {
                const select = document.getElementById('clr');
                select.selectedIndex = i;
                select.dataset.color = colors[i];
            });
        }
    });
}
document.addEventListener('DOMContentLoaded', function () {
    const carousel = document.getElementById('carouselExample');
    const thumbnails = document.querySelectorAll('.pagg-slider img');

    const bsCarousel = bootstrap.Carousel.getOrCreateInstance(carousel, {
        interval: false,
        touch: false
    });

    thumbnails.forEach((thumbnail, index) => {
        thumbnail.addEventListener('click', function () {
            bsCarousel.to(index);
        });
    });
});



 function buttonAddProductToCart(user){
     var addToCartBtn = document.querySelector('.btn-primary');
     addToCartBtn.addEventListener('click', function(event) {
         event.preventDefault();
         var code = addToCartBtn.dataset.code;
         var cantidad = document.getElementById('qty').value;
         var color = document.getElementById('clr').dataset.color;
         var talla = addToCartBtn.dataset.talla;

         if (code !== null && cantidad !== '' && user !== null) {
             if (addToCartBtn.dataset.inCart === '1') {
                 removeProductFromCart(code, user.id, addToCartBtn, color);
             } else {
                 agregarProductoAlCarrito(user.id, code, cantidad, talla, color, addToCartBtn);
             }
         } else {
             console.error('There are missing data to add the product to the cart.');
         }
     });
 }

function getLikesPerProducts(productCode)
{
    return fetch(apiUrl + `PrincipalStruct/getLikesPerProduct/${productCode}`)
    .then(response => response.json())
    .then(data => {
        return data;
    })
    .catch(error => {
        console.error('Error fetching data:', error);
        return Promise.resolve(0);
    });
}

function getProductIndex(productCode) {
    var userCookie = getCookie('user');
    var user;
    if (userCookie) {
        user = JSON.parse(decodeURIComponent(userCookie));
    }
    if(user)
    {
        return fetch(apiUrl + `PrincipalStruct/getSavedProduct/${user.id}/${productCode}`)
        .then(response => response.json())
        .then(data => {
            return data;
        })
        .catch(error => {
            console.error('Error fetching data:', error);
            return Promise.resolve(0);
        });
    }
    else
    {
        return Promise.resolve(0);
    }
}

function alreadyInCart(){
    const addToCartBtn = document.querySelector('.btn-primary');
    const colorSelect = document.getElementById('clr');
    const selectedColor = colorSelect.dataset.color;
    const productCode = addToCartBtn.dataset.code;

    var userCookie = getCookie('user');
    var user;
    if (userCookie) {
        user = JSON.parse(decodeURIComponent(userCookie));
    }

    if(user)
    {
        return fetch(apiUrl + `PrincipalStruct/alreadyInCart/${user.id}/${productCode}/${selectedColor}`)
        .then(response => response.json())
        .then(data => {            
            if(data == 1){
                addToCartBtn.textContent = "Remove from cart";
                addToCartBtn.dataset.inCart = '1';
            }
            else
            {
                addToCartBtn.textContent = "Add to cart";
                addToCartBtn.dataset.inCart = '0';
            }
        })
        .catch(error => {
            console.error('Error fetching data:', error);
            return Promise.resolve(0);
        });
    }
}

function removeProductFromCart(productCode, userId, button, color) {
    fetch(apiUrl + `PrincipalStruct/RemoveProduct/${userId}/${productCode}/${color}`)
    .then(response => {
        if (response.ok) {
            updateCartInHeader();
            button.textContent = "Add to cart";
            button.dataset.inCart = 0;
        } else {
            console.error('Error deleting the product from the cart:', response.statusText);
        }
    })
    .catch(error => {
        console.error('Error deleting the product from the cart:', error);
    });
}

// metodo para agregar productos al carrito
async function agregarProductoAlCarrito(userId, productCode, cantidad, talla, color, addToCartBtn) {
    var api = apiUrl + `PrincipalStruct/addProductToUser/${userId}/${productCode}/${cantidad}/${talla}/${color}`;
    fetch(api, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(function(response) {
            if (response.ok) {
                updateCartInHeader();
                addToCartBtn.textContent = "Remove from cart";
                addToCartBtn.dataset.inCart = 1;
                $('#addProduct').modal('show');
            } else {
                console.error('Error adding product to cart.');
            }
        })
        .catch(function(error) {
            console.error('Error in the application:', error);
            alert('Error in the request. Please try again later.');
        });
}

//cerrar modal de addProduct
document.querySelectorAll('[data-dismiss="modal"]').forEach(button => {
    button.addEventListener('click', () => {
        $('#addProduct').modal('hide');
        $('#modalLogin').modal('hide');
    });
});


function updateCartInHeader(){
    var updateCart = document.getElementById('buttonAddProduct');
    if (updateCart) {
        updateCart.addEventListener('click', function(event) {
            event.preventDefault();
        });
    }
    updateCart.click();
}


    

    // Cargar la calificación existente desde el servidor
    function loadRating() {
        var userCookie = getCookie('user');
        var user;
        if (userCookie) {
            user = JSON.parse(decodeURIComponent(userCookie));
        }

        if(user)
        {
            fetch(`${apiUrl}Opiniones/${code}/${user.id}`)
            .then(response => response.json())
            .then(opinion => {
                const comment = (opinion.comentario && opinion.comentario.length > 0) ? opinion.comentario : '';
                document.getElementById("comment-input").value = comment;

                const rating = (opinion.calificacion && opinion.calificacion.length > 0) ? parseInt(opinion.calificacion[0]) : 0;
                
                for (let i = 0; i < 5; i++) {
                    const star = document.querySelector(`#rating-stars i[data-index="${i + 1}"]`);
                    if (star) {
                        if (i < rating) {
                            star.classList.remove('far');
                            star.classList.add('fas');
                        } else {
                            star.classList.remove('fas');
                            star.classList.add('far');
                        }
                    }
                }
            });
        }
    }
    
    // Manejo de las estrellas de calificación
    document.getElementById("rating-stars").addEventListener("click", (e) => {
        if (e.target.tagName === "I") {
            const rating = parseInt(e.target.getAttribute("data-index"));
            updateRating(rating);
        }
    });
    
    // Actualizar la calificación
    function updateRating(rating) {
        var userCookie = getCookie('user');
        var user;
        if (userCookie) {
            user = JSON.parse(decodeURIComponent(userCookie));
        }

        if(user)
        {
            fetch(`${apiUrl}Opiniones/UpdateRating/${code}/${user.id}/${rating}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(response => response.text())
              .then(data => {
                loadRating();
                loadAllComments();
              });
        }
        else
        {
            localStorage.setItem("currentURL", window.location.href);
            window.location.href = `login.html`;
        }               
    }
    
    
    // Manejo del comentario
    document.getElementById("submit-comment").addEventListener("click", () => {
        const comment = document.getElementById("comment-input").value;
        updateComment(comment);
    });
    
    // Actualizar el comentario
    function updateComment(comment) {
        var userCookie = getCookie('user');
        var user;
        if (userCookie) {
            user = JSON.parse(decodeURIComponent(userCookie));
        }

        if(user)
        {
            fetch(`${apiUrl}Opiniones/UpdateComment/${code}/${user.id}/${comment}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            }).then(response => response.text())
              .then(data => {
                loadRating();
                loadAllComments();
              });
        }
        else
        {
            localStorage.setItem("currentURL", window.location.href);
            window.location.href = `login.html`;
        }  
    }     


    function loadAllComments() {
        fetch(`${apiUrl}Opiniones/${code}`)
            .then(response => response.json())
            .then(opinions => {
                const commentsContainer = document.getElementById('comments-container');
                commentsContainer.innerHTML = '';    
                
                opinions.forEach(opinion => {
                    const userName = opinion.data1[0];
                    const rating = opinion.calificacion[0] || '0';
                    const comment = opinion.comentario[0] || 'No comment provided.';
                    const date = opinion.data4[0];
    
                    const commentBlock = document.createElement('div');
                    commentBlock.classList.add('comment-block');
    
                    const headerContainer = document.createElement('div');
                    headerContainer.style.display = 'flex';
                    headerContainer.style.justifyContent = 'space-between';
                    headerContainer.style.alignItems = 'center'; 

                    const userNameElement = document.createElement('p');
                    userNameElement.innerHTML = `<strong>${userName}</strong>`;

                    const dateElement = document.createElement('p');
                    dateElement.textContent = date;
                    dateElement.style.marginLeft = '10px';

                    headerContainer.appendChild(userNameElement);
                    headerContainer.appendChild(dateElement);
                    commentBlock.appendChild(headerContainer);
    
                    const ratingElement = document.createElement('p');
                    ratingElement.innerHTML = 'Qualification: ';
                
                for (let i = 0; i < rating; i++) {
                    const star = document.createElement('i');
                    star.classList.add('fas', 'fa-star');
                    ratingElement.appendChild(star);
                }
                    //ratingElement.innerHTML = `Qualification: ${rating} <i class="fas fa-star"></i>`;
                    commentBlock.appendChild(ratingElement);
    
                    const commentElement = document.createElement('p');
                    commentElement.textContent = comment;
                    commentBlock.appendChild(commentElement);
    
                    commentsContainer.appendChild(commentBlock);
                });
            })
            .catch(error => {
                console.error('Error fetching comments:', error);
            });
    }



    document.addEventListener("DOMContentLoaded", async () => {
        const ratingSection = document.querySelector(".rating-section");
        var userCookie = getCookie('user');
        var user;

        if (userCookie) {
            user = JSON.parse(decodeURIComponent(userCookie));
        }

        if(user)
        {
            const isEnable = await checkCommentsVisibility(code, user.Id);
        
            if (isEnable) {
                ratingSection.style.display = "block";
            } else {
                ratingSection.style.display = "none";
            }
        }
        else
        {
            ratingSection.style.display = "none";
        }
    });

    async function checkCommentsVisibility(codigo, userId) {
        try {
            const response = await fetch(`${apiUrl}Opiniones/enableComments/${codigo}/${userId}`);
            if (!response.ok) {
                console.error("Error fetching data:", response.statusText);
                return false;
            }
            const isEnable = await response.json();
            return isEnable;
        } catch (error) {
            console.error("Error:", error);
            return false;
        }
    }
    
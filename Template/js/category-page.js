

const apiUrl = apiUrl3;
let categoryName = "Todo";
let subCategoryName = null;

var numPage = 0;

document.addEventListener('DOMContentLoaded', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const searchBy = urlParams.get('searchBy');
    const category = urlParams.get('category');
    const subcategory = urlParams.get('subcategory') ?? null; // Obtener la subcategoría
    if (category) {
        categoryName = category;
    }

    if (subcategory) {
        subCategoryName = subcategory;
    }
    if (searchBy) {
         getBySearch(searchBy);
    } else {
        loadProducts(categoryName, subCategoryName, 1);
        loadPageIndices(categoryName, subCategoryName); 
    }

    const categoryAll = document.getElementById("all");
    categoryAll.addEventListener("click", function (event) {
        categoryName = "Todo";
        loadProducts(categoryName, null, 1);
        loadPageIndices(categoryName, null);
    });
});

function loadProducts(category, subcategory, page) {
    const endpoint = subcategory
        ? `PrincipalStruct/getProductsPerPage/${category}/${page}/${subcategory}`
        : `PrincipalStruct/getProductsPerPage/${category}/${page}`;

    fetch(apiUrl1 + endpoint)
        .then(response => response.json())
        .then(data => {
            const productGrid = document.getElementById('productGrid');
            productGrid.innerHTML = '';

            data.nombre.forEach((nombre, index) => {
                if (!data.precio[index] || !data.images[index] || !data.codigo[index]) {
                    return;
                }

                const precio = data.precio[index];
                const codigo = data.codigo[index];
                const imagen = data.images[index][1];
                const discountPrice = data.extra8[index] === "1" ? data.extra7[index] : precio;

                const productDiv = document.createElement('div');
                productDiv.className = 'product-block';
                productDiv.id = `product${index + 1}a`;

                productDiv.innerHTML = `
                    <div class="over" id="over${index + 1}">
                        <div class="align-left">
                            <strong class="title-name">
                                <a class="redirect" href="product-page.html">${nombre}</a>
                            </strong>
                            <div style="display: flex; flex-direction: row; gap: 5px;">
                                <strong class="price"><del class="descuento">${data.extra8[index] === "1" ? `${precio}$` : ''}</del></strong>
                                <strong class="price precio">${discountPrice}$</strong>
                            </div>
                        </div>
                        <a href="#" class="like text-center">
                            <i class="icon-favorite" data-code="${codigo}"></i>
                            <p class="likes">0</p>
                        </a>
                    </div>
                    <img class="img-responsive" alt="image description" src="${imagen}">
                `;
                productGrid.appendChild(productDiv);

                const iconFavorite = productDiv.querySelector('.icon-favorite');
                const like = productDiv.querySelector('.likes');

                getProductIndex(codigo).then(result => {
                    iconFavorite.dataset.favorite = result;
                    if (iconFavorite.dataset.favorite === '1') {
                        iconFavorite.classList.add('favorite');
                    } else {
                        iconFavorite.classList.remove('favorite');
                    }
                });
                
                getLikesPerProducts(codigo).then(result => {
                    iconFavorite.dataset.likes = result;
                    like.textContent = iconFavorite.dataset.likes;
                });

                productDiv.addEventListener('click', () => {
                    redirectProduct(codigo);
                });
            });
        })
        .catch(error => console.error('Error fetching data:', error));
}


function redirectProduct(codigo) {
    event.preventDefault();
    event.stopPropagation();

    window.location.href = `product-page.html?code=${codigo}`;
}

function getLikesPerProducts(productCode)
{
    return fetch(apiUrl1 + `PrincipalStruct/getLikesPerProduct/${productCode}`)
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
        return fetch(apiUrl1 + `PrincipalStruct/getSavedProduct/${user.id}/${productCode}`)
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



function loadPageIndices(category, subcategory = null) {
    const url = subcategory
        ? `${apiUrl1}PrincipalStruct/getPages/${category}/${subcategory}`
        : `${apiUrl1}PrincipalStruct/getPages/${category}`;

    fetch(url)
        .then(response => response.json())
        .then(totalPages => {
            renderPagination(totalPages, category, subcategory);
        })
        .catch(error => console.error('Error fetching pages:', error));
}

function renderPagination(totalPages, category, subcategory) {
    const paginationHolder = document.querySelector('.pagination');
    paginationHolder.innerHTML = '';

    const prevLi = document.createElement('li');
    prevLi.classList.add('page-item');
    prevLi.innerHTML = `<a class="page-link" href="#">&#8249;</a>`;
    paginationHolder.appendChild(prevLi);
   
    prevLi.addEventListener('click', () => {
        const activePage = document.querySelector('.pagination .active');
        if (activePage && activePage.previousElementSibling) {
            const prevPage = activePage.previousElementSibling.querySelector('.page-link');
            if (prevPage) prevPage.click();
        }
    });

    for (let i = 1; i <= totalPages; i++) {
        const pageLi = document.createElement('li');
        pageLi.classList.add('page-item');
        if (i === 1) pageLi.classList.add('active');
        pageLi.innerHTML = `<a class="page-link" href="#">${i}</a>`;
        paginationHolder.appendChild(pageLi);

        pageLi.addEventListener('click', () => {
            document.querySelectorAll('.pagination .page-item').forEach(item => item.classList.remove('active'));
            pageLi.classList.add('active');
            loadProducts(category, subcategory, i);
        });
    }

    const nextLi = document.createElement('li');
    nextLi.classList.add('page-item');
    nextLi.innerHTML = `<a class="page-link" href="#">&#8250;</a>`;
    paginationHolder.appendChild(nextLi);

    nextLi.addEventListener('click', () => {
        const activePage = document.querySelector('.pagination .active');
        if (activePage && activePage.nextElementSibling) {
            const nextPage = activePage.nextElementSibling.querySelector('.page-link');
            if (nextPage) nextPage.click();
        }
    });
}

function getBySearch(searchText) {
    fetch(apiUrl + `PrincipalStruct/getBySearch/${searchText}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al obtener los datos');
            }
            return response.json();
        })
        .then(data => {
            const productGrid = document.getElementById('productGrid');
            productGrid.innerHTML = '';
console.log("respuesta",data);
            data.nombre.forEach((nombre, index) => {
                if (!data.precio[index] || !data.images[index] || !data.codigo[index]) {
                    return;
                }

                const precio = data.precio[index];
                const codigo = data.codigo[index];
                const imagen = data.images[index][1];
                const discountPrice = data.extra8[index] === "1" ? data.extra7[index] : precio;

                const productDiv = document.createElement('div');
                productDiv.className = 'product-block';
                productDiv.id = `product${index + 1}a`;

                productDiv.innerHTML = `
                    <div class="over" id="over${index + 1}">
                        <div class="align-left">
                            <strong class="title-name">
                                <a class="redirect" href="product-page.html">${nombre}</a>
                            </strong>
                            <div style="display: flex; flex-direction: row; gap: 5px;">
                                <strong class="price"><del class="descuento">${data.extra8[index] === "1" ? `${precio}$` : ''}</del></strong>
                                <strong class="price precio">${discountPrice}$</strong>
                            </div>
                        </div>
                        <a href="#" class="like text-center">
                            <i class="icon-favorite" data-code="${codigo}"></i>
                            <p class="likes">0</p>
                        </a>
                    </div>
                    <img class="img-responsive" alt="image description" src="${imagen}">
                `;

                productGrid.appendChild(productDiv);

                const iconFavorite = productDiv.querySelector('.icon-favorite');
                const like = productDiv.querySelector('.likes');

                getProductIndex(codigo).then(result => {
                    iconFavorite.dataset.favorite = result;
                    if (iconFavorite.dataset.favorite === '1') {
                        iconFavorite.classList.add('favorite');
                    } else {
                        iconFavorite.classList.remove('favorite');
                    }
                });

                getLikesPerProducts(codigo).then(result => {
                    iconFavorite.dataset.likes = result;
                    like.textContent = iconFavorite.dataset.likes;
                });

                productDiv.addEventListener('click', () => {
                    redirectProduct(codigo);
                });
            });
        })
        .catch(error => {
            console.error('Error en la solicitud:', error);
        });
}

//********************************SCROLL VIEW */    

let lastKnownScrollPosition = 0;
let ticking = false;



document.addEventListener("scroll", (event) => {
  lastKnownScrollPosition = window.scrollY;

  if (!ticking) {
    window.requestAnimationFrame(() => {
      doSomething(lastKnownScrollPosition);
      ticking = false;
    });

    ticking = true;
  }
});





function doSomething(scrollPos) {
    // Do something with the scroll position
    //console.log(scrollPos)
    var elem1 = document.getElementById('product1a');
    var elem2 = document.getElementById('product2a');
    var elem3 = document.getElementById('product3a');
    var elem4 = document.getElementById('product4a');
    var elem5 = document.getElementById('product5a');
    var elem6 = document.getElementById('product6a');
    var elem7 = document.getElementById('product7a');
    var elem8 = document.getElementById('product8a');
    var elem9 = document.getElementById('product9a');
    var elem10 = document.getElementById('product10a');
    
    if (isInViewport(elem1)) {
        // está en el viewport
        //console.log("si esta")
        var chang1 = document.getElementById('over1');
        ChangeViewVisible(chang1);
        
    }
    else{
        var chang1a= document.getElementById('over1');
        ChangeViewHidenn(chang1a);
    }

    if (isInViewport(elem2)) {
        // está en el viewport
        //console.log("si esta")
        var chang2 = document.getElementById('over2');
        ChangeViewVisible(chang2);
        
    }
    else{
        var chang2 = document.getElementById('over2');
        ChangeViewHidenn(chang2);
    }

    if (isInViewport(elem3)) {
        // está en el viewport
        //console.log("si esta")
        var chang3 = document.getElementById('over3');
        ChangeViewVisible(chang3);
        
    }
    else{
        var chang3 = document.getElementById('over3');
        ChangeViewHidenn(chang3);
    }

    if (isInViewport(elem4)) {
        // está en el viewport
        //console.log("si esta")
        var chang4 = document.getElementById('over4');
        ChangeViewVisible(chang4);
        
    }
    else{
        var chang4 = document.getElementById('over4');
        ChangeViewHidenn(chang4);
    }

    if (isInViewport(elem5)) {
        // está en el viewport
        //console.log("si esta")
        var chang5 = document.getElementById('over5');
        ChangeViewVisible(chang5);
        
    }
    else{
        var chang5 = document.getElementById('over5');
        ChangeViewHidenn(chang5);
    }

    if (isInViewport(elem6)) {
        // está en el viewport
        //console.log("si esta")
        var chang6 = document.getElementById('over6');
        ChangeViewVisible(chang6);
        
    }
    else{
        var chang6 = document.getElementById('over6');
        ChangeViewHidenn(chang6);
    }

    if (isInViewport(elem7)) {
        // está en el viewport
        //console.log("si esta")
        var chang7 = document.getElementById('over7');
        ChangeViewVisible(chang7);
        
    }
    else{
        var chang7 = document.getElementById('over7');
        ChangeViewHidenn(chang7);
    }

    if (isInViewport(elem8)) {
        // está en el viewport
        //console.log("si esta")
        var chang8 = document.getElementById('over8');
        ChangeViewVisible(chang8);
        
    }
    else{
        var chang8 = document.getElementById('over8');
        ChangeViewHidenn(chang8);
    }

    if (isInViewport(elem9)) {
        // está en el viewport
        //console.log("si esta")
        var chang9 = document.getElementById('over9');
        ChangeViewVisible(chang9);
        
    }
    else{
        var chang9 = document.getElementById('over9');
        ChangeViewHidenn(chang9);
    }

    if (isInViewport(elem10)) {
        // está en el viewport
        //console.log("si esta")
        var chang10 = document.getElementById('over10');
        ChangeViewVisible(chang10);
        
    }
    else{
        var chang10 = document.getElementById('over10');
        ChangeViewHidenn(chang10);
    }
    
  }

  function isInViewport(elem) {
    var distance = elem.getBoundingClientRect();
    return (
        ((window.innerHeight || document.documentElement.clientHeight)/2)+distance.top < (window.innerHeight || document.documentElement.clientHeight) && distance.bottom > 500
    );
}


function ChangeViewVisible(changB){
    changB.style.display = "block";
    changB.style.visibility = "visible";
    changB.style.transform = "translateY(0)";
    changB.style.opacity = "0.6";
}

function ChangeViewHidenn(changA){
        changA.style.opacity = "1";
        changA.style.visibility = "visible";
        changA.style.transform = "translateY(100%)";
        changA.style.transition = "all .25s linear";
        changA.style.position = "absolute";
        changA.style.left = "0px";
        changA.style.bottom = "0px";
        changA.style.right = "0px";
        changA.style.padding = "44px 30px 28px";
}






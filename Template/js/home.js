const apiUrl = apiUrl3;

document.addEventListener('DOMContentLoaded', loadProducts);
function loadProducts(){
    fetch(apiUrl + `PrincipalStruct/getProductsPerPromo`)
    .then(response => response.json())
    .then(data => {
        const productBlocks = document.querySelectorAll(".product-block2");

        productBlocks.forEach((productBlock, index) => {
            if(index < data.nombre.length)
            {
                const nombre = data.nombre[index];
                const precio = data.extra7[index];
                
                let imagenIndex;
                if (index === 2) {
                    imagenIndex = 4; // ft5 960x1235
                } else if (index === 3) {
                    imagenIndex = 0; // ft1 960x620
                } else if(index == 0)
                {
                    imagenIndex = 1; // ft4 390x330
                } else {
                    imagenIndex = 1; // ft2 480x620
                }
                
                const imagen = data.images[index][imagenIndex];
                const titleName = productBlock.querySelector(".title-name");
                //titleName.textContent = nombre;
                titleName.textContent = "";
                const like = productBlock.querySelector(".likes");
                const iconFavorite = productBlock.querySelector(".icon-favorite");
                
                if(iconFavorite)
                {
                    iconFavorite.dataset.code = data.codigo[index];

                    getProductIndex(data.codigo[index]).then(result => {
                        iconFavorite.dataset.favorite = result;
                        
                        if (iconFavorite.dataset.favorite === '1') {
                            iconFavorite.classList.add('favorite');
                        } else {
                            iconFavorite.classList.remove('favorite');
                        }
                    });

                    getLikesPerProducts(data.codigo[index]).then(result => {
                        iconFavorite.dataset.likes = result;
                        like.textContent = iconFavorite.dataset.likes;
                    })
                }
                const delElement = productBlock.querySelector(".descuento");
                if(delElement)
                {
                    if (data.extra8[index] === "1") {
                        delElement.textContent = data.precio[index] + "$.";
                    } else {
                        delElement.textContent = "";
                    }
                }

                const comprar = productBlock.querySelector(".comprar");
                if(comprar)
                {
                    comprar.textContent = data.categoria[index];
                    comprar.dataset.categoria = data.categoria[index];
                    comprar.dataset.codigo = data.codigo[index];
                    productBlock.dataset.categoria = data.categoria[index];
                    productBlock.dataset.codigo = data.codigo[index];
                    
                    productBlock.addEventListener('click', () => redirectProduct(event));
                    comprar.addEventListener('click', () => redirectProduct(event));
                }
                
                const categoryBlock = productBlock.querySelector(".category");
                if (categoryBlock) {
                    categoryBlock.textContent = data.categoria[index];
                }
                
                const price = productBlock.querySelector(".precio");
                const img = productBlock.querySelector(".img-primary");
                const img2 = productBlock.querySelector(".img-secondary")
                price.textContent = precio + "$.";
                img.src = imagen;
                console.log("image: ", data.images[index][imagenIndex]);
                console.log("image: ", data.images[index][0]);
                img2.src = data.images[index][0];
                // if(index == 0)
                // {
                //     const description = productBlock.querySelector(".description");
                //     const img = productBlock.querySelector(".image-ft4");

                //     description.textContent = data.descripcion[index];
                //     img.src = imagen;
                // }
                // else
                // {
                //     const price = productBlock.querySelector(".precio");
                //     const img = productBlock.querySelector(".img-primary");
                //     const img2 = productBlock.querySelector(".img-secondary")
                //     price.textContent = precio + "$.";
                //     img.src = imagen;
                //     img2.src = data.images[index][0];
                // }
            }
        });
    })
    .catch(error => console.error('Error fetching data:', error));
}

function redirectProduct(event) {
    event.preventDefault();
    event.stopPropagation();

    const codigo = event.currentTarget.dataset.codigo;
    console.log(codigo)
    const categoria = event.currentTarget.dataset.categoria;

    fetch(apiUrl + `PrincipalStruct/getPagePerCategory/${codigo}`)
    .then(response => response.json())
    .then(positions => {
        window.location.href = `product-page.html?code=${codigo}`; 
    })
    .catch(error => console.error('Error al obtener la pagina del producto:', error));

    //window.location.href = apiUrl+ `product-page.html?code=${codigo}`; 
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

  document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.getElementById('searchForm');

    searchForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const searchText = document.getElementById('search').value;

        window.location.href = `/category-page.html?searchBy=${encodeURIComponent(searchText)}`;
    });
});


//contador de visitas
window.onload = function() {
    fetch(apiUrl + `Visitas`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    })
    .then(response => response.json())
    .catch(error => console.error('Error al actualizar visitas:', error));
};




//**************************************************Scroll View */
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

    
    if (isInViewport(elem1)) {
        // está en el viewport
        //console.log("si esta")
        var chang1 = document.getElementById('over1');
        var cat1 = document.getElementById('cat1');
        ChangeViewVisible(chang1);
        ChangeViewCategoryVisible(cat1);
        
    }
    else{
        var chang1a= document.getElementById('over1');
        var cat1 = document.getElementById('cat1');
        ChangeViewHidenn(chang1a);
        ChangeViewCategoryHidenn(cat1);
    }

    if (isInViewport(elem2)) {
        // está en el viewport
        //console.log("si esta")
        var chang2 = document.getElementById('over2');
        var cat2 = document.getElementById('cat2');
        ChangeViewVisible(chang2);
        ChangeViewCategoryVisible(cat2);
        
        
    }
    else{
        var chang2 = document.getElementById('over2');
        var cat2 = document.getElementById('cat2');
        ChangeViewHidenn(chang2);
        ChangeViewCategoryHidenn(cat2);
    }

    if (isInViewport(elem3)) {
        // está en el viewport
        //console.log("si esta")
        var chang3 = document.getElementById('over3');
        var cat3 = document.getElementById('cat3');
        ChangeViewVisible(chang3);
        ChangeViewCategoryVisible(cat3);
        
    }
    else{
        var chang3 = document.getElementById('over3');
        var cat3 = document.getElementById('cat3');
        ChangeViewHidenn(chang3);
        ChangeViewCategoryHidenn(cat3);
    }

    if (isInViewport(elem4)) {
        // está en el viewport
        //console.log("si esta")
        var chang4 = document.getElementById('over4');
        var cat4 = document.getElementById('cat4');
        ChangeViewVisible(chang4);
        ChangeViewCategoryVisible(cat4);
    }
    else{
        var chang4 = document.getElementById('over4');
        var cat4 = document.getElementById('cat4');
        ChangeViewHidenn(chang4);
        ChangeViewCategoryHidenn(cat4);
    }

    if (isInViewport(elem5)) {
        // está en el viewport
        //console.log("si esta")
        var chang5 = document.getElementById('over5');
        var cat5 = document.getElementById('cat5');
        ChangeViewVisible(chang5);
        ChangeViewCategoryVisible(cat5);
        
    }
    else{
        var chang5 = document.getElementById('over5');
        var cat5 = document.getElementById('cat5');
        ChangeViewHidenn(chang5);
        ChangeViewCategoryHidenn(cat5);
    }

    if (isInViewport(elem6)) {
        // está en el viewport
        //console.log("si esta")
        var chang6 = document.getElementById('over6');
        var cat6 = document.getElementById('cat6');
        ChangeViewVisible(chang6);
        ChangeViewCategoryVisible(cat6);
    }
    else{
        var chang6 = document.getElementById('over6');
        var cat6 = document.getElementById('cat6');
        ChangeViewHidenn(chang6);
        ChangeViewCategoryHidenn(cat6);
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

function ChangeViewCategoryVisible(changB){
    changB.style.display = "block";

}

function ChangeViewCategoryHidenn(changA){
        changA.style.display = "none";
}


document.addEventListener("DOMContentLoaded", function () {
    var discountModal = new bootstrap.Modal(document.getElementById('discountModal'));
    discountModal.show();
});

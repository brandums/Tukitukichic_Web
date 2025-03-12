const apiUrl1 = apiUrl3;

const userName = document.getElementById('userName');
const userName2 = document.getElementById('userName2');
const loginUser = document.getElementById('iconUser');
const logout = document.getElementById('logout');
var user;

document.addEventListener("DOMContentLoaded", function () {
    const menuStore = document.querySelector('.mega-drop .drop-holder');
    const menuToggle = document.querySelector('#menu-toggle');
    const storeLink = document.querySelector('.mega-drop > a');
    const userName2 = document.querySelector('#userName2');
    let isMobile = false;
  
    function checkScreenSize() {
        if (window.innerWidth <= 768) {
            isMobile = true;
            if (menuStore) {
                menuStore.style.display = 'block';
            }
        } else {
            isMobile = false;
            if (menuStore) {
                menuStore.style.display = 'none';
            }
        }

        if (window.innerWidth <= 768) {
            userName2.style.display = "block";
            userName2.textContent = user.name.split(' ')[0];
        } else {
            userName2.style.display = "none";
        }
    }
  
    window.addEventListener('resize', checkScreenSize);
    checkScreenSize();
  
    if (isMobile) {
        menuToggle.addEventListener('click', function () {
            if (menuStore) {
                menuStore.style.display = (menuStore.style.display === 'block') ? 'none' : 'block';
            }
        });
    }
  
    if (!isMobile) {
        if (storeLink && menuStore) {
            storeLink.addEventListener('mouseenter', function () {
                menuStore.style.display = 'block';
            });

            storeLink.addEventListener('mouseleave', function () {
                setTimeout(() => {
                    if (!menuStore.matches(':hover') && !storeLink.matches(':hover')) {
                        menuStore.style.display = 'none';
                    }
                }, 200);
            });

            menuStore.addEventListener('mouseenter', function () {
                menuStore.style.display = 'block';
            });

            menuStore.addEventListener('mouseleave', function () {
                setTimeout(() => {
                    if (!storeLink.matches(':hover')) {
                        menuStore.style.display = 'none';
                    }
                }, 200);
            });
        }
    }
});

  

document.addEventListener('DOMContentLoaded', async function() {
    await createInstance();
    checkAuthentication();
	getCategories();
});

document.querySelector('.cart-opener').addEventListener('click', function(event) {
    event.preventDefault();
});

async function createInstance(){
	fetch(apiUrl1 + 'PrincipalStruct/PostPrincipalStruct', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		}
	});
}

function checkAuthentication() {
    var userCookie = getCookie('user');
    if (userCookie) {
        user = JSON.parse(decodeURIComponent(userCookie));
	
		userName.style.display = "block";
		userName.textContent = user.name.split(' ')[0];

		if (window.innerWidth <= 768) {
			userName2.style.display = "block";
			userName2.textContent = user.name.split(' ')[0];
		}
		
		loginUser.style.display = "none";
		logout.style.display = "block";

    } else {
		loginUser.style.display = "block";
		userName.style.display = "none";
		userName2.style.display = "none";
		logout.style.display = "none";
    }
}

//obtener cookies
function getCookie(name) {
    var cookies = document.cookie.split(';');
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].trim();
        if (cookie.startsWith(name + '=')) {
            return cookie.substring(name.length + 1);
        }
    }
    return null;
}

//cerrar sesion
document.addEventListener('DOMContentLoaded', function() {
    var logoutButton = document.querySelector('.logout-btn');

    logoutButton.addEventListener('click', function(event) {
        event.preventDefault();

        document.cookie = "user=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
		userName.style.display = "none";
		loginUser.style.display = "block";
		logout.style.display = "none";

        window.location.href = 'index.html';
    });
});


function getCategories() {
	const categoryList = document.querySelector('.category-list');
  
	fetch(apiUrl1 + 'PrincipalStruct/getCategory')
	  .then(response => response.json())
	  .then(data => {
		data.forEach(categoria => {
		  const listItem = document.createElement('li');
		  const link = document.createElement('a');
		  link.textContent = categoria.name;
		  link.href = `/category-page.html?category=${categoria.name}`;
  
		  if (categoria.subCategories && categoria.subCategories.length > 0) {
			const subCategoryList = document.createElement('ul');
			subCategoryList.classList.add('sub-category-list');
			subCategoryList.style.display = 'none';
  
			categoria.subCategories.forEach(subCategoria => {
			  if (subCategoria.isEnabled) {
				const subCategoryItem = document.createElement('li');
				const subCategoryLink = document.createElement('a');
				subCategoryLink.textContent = subCategoria.name;
				subCategoryLink.href = `/category-page.html?category=${categoria.name}&subcategory=${subCategoria.name}`;
				subCategoryItem.appendChild(subCategoryLink);
				subCategoryList.appendChild(subCategoryItem);
			  }
			});
  
			const toggleButton = document.createElement('button');
			toggleButton.textContent = '►';
			toggleButton.classList.add('toggle-subcategories');
			toggleButton.addEventListener('click', (event) => {
			  event.stopPropagation();
  
			  const isExpanded = subCategoryList.style.display === 'block';
			  subCategoryList.style.display = isExpanded ? 'none' : 'block';
			  toggleButton.classList.toggle('open', !isExpanded);
			});
  
			link.addEventListener('click', (event) => {
			  event.preventDefault();
			  const isExpanded = subCategoryList.style.display === 'block';
			  subCategoryList.style.display = isExpanded ? 'none' : 'block';
			  toggleButton.classList.toggle('open', !isExpanded);
			});
  
			listItem.appendChild(link);
			listItem.appendChild(toggleButton);
			listItem.appendChild(subCategoryList);
		  } else {
			listItem.appendChild(link);
		  }
  
		  categoryList.appendChild(listItem);
		});
	  })
	  .catch(error => {
		console.error('Error al obtener las categorías:', error);
	  });
  }
  



function listCategories(data){
	const filterList = document.querySelector('.filter-list');

	if(filterList)
		{
			data.forEach(category => {
				const listItem = document.createElement('li');
				const link = document.createElement('a');
				link.href = '#';
				link.textContent = category;
				link.addEventListener('click', function(event) {
					categoryName = category;
					loadPageIndices(categoryName);
					loadProducts(categoryName, 1);
				});
				listItem.appendChild(link);
				filterList.appendChild(listItem);
			});
		}
}



  var productosEnCarrito = [];
	document.addEventListener('DOMContentLoaded', function() {
		var cartList = document.querySelector('.cart-list');
	
		// Función para eliminar un producto del carrito
		function removeProductFromCart(productCode, color) {
			fetch(apiUrl1 + `PrincipalStruct/RemoveProduct/${user.id}/${productCode}/${color}`)
			.then(response => {
				if (response.ok) {
					updateCart();
					addToCartBtn = document.querySelector('.btn-primary');
					if(addToCartBtn)
					{
						addToCartBtn.dataset.inCart = 0;
						addToCartBtn.textContent = "agregar al carrito";
					}
				} else {
					console.error('Error al eliminar el producto del carrito:', response.statusText);
				}
			})
			.catch(error => {
				console.error('Error al eliminar el producto del carrito:', error);
			});
		}
	
		// Función para actualizar el carrito
		function updateCart() {
			if(user && user.id !== 1)
			{
				fetch(apiUrl1 + `PrincipalStruct/substruct/${user.id}`)
				.then(response => response.json())
				.then(data => {
					productosEnCarrito = data;
					cartList.innerHTML = '';

					fillShopingCart(data);
	
					const cantidad = document.getElementById('cantidad');
					const iconCantidad = document.getElementById('iconCantidad');
					const total = document.getElementById('total');

					if(data.nombre.length == 1 && data.nombre[0] == "" || data.nombre[0] == null)
					{
						cantidad.textContent = 0;
						iconCantidad.textContent = 0;
					}
					else{
						cantidad.textContent = `${data.nombre.length} items`;
						iconCantidad.textContent = data.nombre.length;
					}
		
					var totalPrice = 0;

					data.nombre.forEach((product, index) => {
						var listItem = document.createElement('li');
						listItem.innerHTML = `
							<div class="image">
								<a href="#"><img alt="Image Description" src="${data.images[index][0]}"></a>
							</div>
							<div class="description">
								<span class="product-name"><a href="#">${product}</a></span>
								<span class="price">${data.precio[index]} $.</span>
							</div>
							<a class="icon-close" href="#" data-user-id="${user.id}" data-product-index="${data.codigo[index]}" data-product-color="${data.color[index]}"></a>
						`;
						cartList.appendChild(listItem);
		
						totalPrice += parseFloat(data.precio[index] * data.extra1[index]);
					});
					total.textContent = `${totalPrice} $.`;
				})
			.catch(error => {
				console.error('Error al obtener los productos del carrito:', error);
			});
			}
		}

		function fillShopingCart(producto){
			const detailContainer = document.querySelector('.products-container');
			let totalPrice = 0;
			
			if(detailContainer){
				detailContainer.innerHTML = '';
				producto.nombre.forEach((product, index) => {
					const detailDiv = document.createElement('div');
					detailDiv.classList.add('row', 'wow', 'fadeInUp');
					detailDiv.setAttribute('data-wow-delay', '0.4s');

					let totalProduct = parseFloat(producto.precio[index] * producto.extra1[index]);

					detailDiv.innerHTML = `
						<div class="detail-holder">
							<div class="col-xs-12 col-sm-2">
								<div class="img-holder">
									<img src="${producto.images[index]}" alt="image description">
								</div>
							</div>
							<div class="col-xs-12 col-sm-3" style="display:flex; flex-direction: column;">
								<span class="txt" style="margin-top:15px; padding:0px;">${product}</span>
								<span class="txt" style="margin-top:15px; padding:0px;">${producto.extra2[index]}: ${producto.talla[index]}</span>
								<span class="txt" style="margin-top:15px; padding:0px;">Color: ${producto.color[index]}</span>
							</div>
							<div class="col-xs-12 col-sm-2">
								<span class="txt">${producto.precio[index]} $.</span>
							</div>
							<div class="col-xs-12 col-sm-3">
								<span class="qynt">${producto.extra1[index]}</span>
							</div>
							<div class="col-xs-12 col-sm-2">
								<span class="txt">${totalProduct} $.</span>
							</div>
						</div>
					`;
					detailContainer.appendChild(detailDiv);
			
					const hrElement = document.createElement('hr');
					detailContainer.appendChild(hrElement);

					totalPrice += totalProduct;
				});

				const shippingPriceElement = document.getElementById('shippingPrice');
				let currentPriceText = shippingPriceElement.textContent.trim();
				let currentPrice = parseFloat(currentPriceText.replace(/[^0-9.-]+/g,""));
				let totalPriceFinal = currentPrice + totalPrice;

				document.querySelector(".finalPrice").textContent = totalPriceFinal + " $.";
				document.querySelector(".finalPrice").dataset.basePrice = totalPriceFinal;
			}
		}
	
		cartList.addEventListener('click', function(event) {
			if (event.target.classList.contains('icon-close')) {
				event.preventDefault();
				var productCode = event.target.getAttribute('data-product-index');
				var color = event.target.getAttribute('data-product-color');
				if (user.id && productCode && color) {
					removeProductFromCart(productCode, color);
				}
			}
		});

		var addToCartBtn = document.getElementById('buttonAddProduct');
		if(addToCartBtn)
		{
			addToCartBtn.addEventListener('click', function(event){
				event.preventDefault();
				updateCart();
			})
		}	

		updateCart();
	});

	document.addEventListener('DOMContentLoaded', function() {
		var buyButton = document.getElementById('pagar');
		if(buyButton)
		{
			buyButton.addEventListener('click', function(event) {
				event.preventDefault();

				var productos = productosEnCarrito.nombre.map((producto, index) => ({
					name: productosEnCarrito.nombre[index],
					price: productosEnCarrito.precio[index],
					quantity: productosEnCarrito.extra1[index],
				}));

				const address = document.getElementById('address');
				const countrySelect = document.getElementById('country');
				const shippingPriceElement = document.getElementById('shippingPrice');
				let currentPriceText = shippingPriceElement.textContent.trim();
        		let currentPrice = parseFloat(currentPriceText.replace(/[^0-9.-]+/g,""));

				fetch(apiUrl1 + `Payment/create-checkout-session`, {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						items: productos,
        				userId: user.id,
						shipping: currentPrice,
						address: address.value,
						country: countrySelect.value
					})
				})
				.then(response => {
					if (response.ok) {
						return response.json();
					} else {
						throw new Error('La solicitud falló con un código de estado ' + response.status);
					}
				})
				.then(data => {
					var stripe = Stripe('pk_key');
					return stripe.redirectToCheckout({ sessionId: data.id });
				})
				.then(result => {
					if (result.error) {
						console.error('Error en la redirección:', result.error.message);
					}
				})
				.catch(error => {
					console.error('Error al realizar la compra:', error);
				});
			});
		}
	});

	function realizarCompra() {
		fetch(apiUrl1 + `PrincipalStruct/buyProducts/${user.id}`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
			})
		})
		.then(response => {
			if (response.ok) {
				return response.json();
			} else {
				throw new Error('La solicitud falló con un código de estado ' + response.status);
			}
		})
		.catch(error => {
			console.error('Error al realizar la compra:', error);
		});
	}
	
	
document.addEventListener('DOMContentLoaded', function(){
	var addFavoriteBtns = document.querySelectorAll('.icon-favorite');
    if (addFavoriteBtns) {
        addFavoriteBtns.forEach(function(addFavoriteBtn) {
            addFavoriteBtn.addEventListener('click', function(event) {
                event.preventDefault();
                var productCode = addFavoriteBtn.dataset.code;

                var userCookie = getCookie('user');
                var user;
                var userId;
                if (userCookie) {
                    user = JSON.parse(decodeURIComponent(userCookie));
                    userId = user.id;
                }

				if(user){
					if (addFavoriteBtn.dataset.favorite == 1) {
                        eliminarProductoDeFavoritos(userId, productCode, addFavoriteBtn);
                    } else {
                        agregarProductoAFavoritos(userId, productCode, addFavoriteBtn);
                    }
				}
				else{
					window.location.href = `login.html`;
				}
            });
        });
    }
});

// metodo para agregar productos a favoriitos
function agregarProductoAFavoritos(userId, productCode, addFavoriteBtn) {
    var api = apiUrl1 + `PrincipalStruct/addFavoriteProduct/${userId}/${productCode}`;
    fetch(api, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(function(response) {
            if (response.ok) {
				addFavoriteBtn.classList.add('favorite');
				addFavoriteBtn.dataset.favorite = 1;
				const likeElement = addFavoriteBtn.closest('.like').querySelector('.likes');
				if(likeElement)
				{
					likeElement.textContent = parseInt(addFavoriteBtn.dataset.likes) + 1;
					addFavoriteBtn.dataset.likes = likeElement.textContent;
				}
            } else {
                console.error('Error al agregar el producto a favoritos.');
            }
        })
        .catch(function(error) {
            console.error('Error en la solicitud:', error);
        });
}

// metodo para eliminar productos de favoriitos
function eliminarProductoDeFavoritos(userId, productCode, addFavoriteBtn) {
    var api = apiUrl1 + `PrincipalStruct/deleteFavoriteProduct/${userId}/${productCode}`;
    fetch(api, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(function(response) {
            if (response.ok) {
				addFavoriteBtn.classList.remove('favorite');
				addFavoriteBtn.dataset.favorite = 0;
				const likeElement = addFavoriteBtn.closest('.like').querySelector('.likes');
				likeElement.textContent = parseInt(addFavoriteBtn.dataset.likes) - 1;
				addFavoriteBtn.dataset.likes = likeElement.textContent;
            } else {
                console.error('Error al eliminar el producto de favoritos.');
            }
        })
        .catch(function(error) {
            console.error('Error en la solicitud:', error);
        });
}
	


// page init
jQuery(function(){
	"use strict";

	initIsoTop();
	initbackTop();
	initCounter();
	initAddClass();
	initCountDown();
	initSlickSlider();
	initStickyHeader();
});

jQuery(window).on('load', function() {
	"use strict";

	initIsoTop();
	initPreLoader();
	new WOW().init();
	initStyleChanger();
});



// count down init
function initCountDown() {
	var newDate = new Date(2018, 12, 28);
	
	jQuery("#defaultCountdown").countdown({until: newDate});
}

function initAddClass() {
	jQuery(".btn-search, .close-btn").click(function() {
		jQuery("body").toggleClass("search-active");
	});
	jQuery(".nav-opener").click(function() {
		jQuery(".nav-holder").toggleClass("nav-active");
	});
	jQuery(".cart-box").click(function() {
		jQuery(".icon-list").toggleClass("cart-active");
	});
}

function initSlickSlider() {
	jQuery('#main-slider').slick({
		dots: true,
		arrows: false,
		autoplay: true,
		speed: 800,
		infinite: true,
		adaptiveHeight: true,
		fade: true,
		autoplaySpeed: 4000
	});
	jQuery('.blockquote-slider').slick({
		autoplay: true,
		adaptiveHeight: true
	});
	jQuery('.team-slider').slick({
		dots: true,
		autoplay: true,
		slidesToShow: 3,
		slidesToScroll: 1,
		adaptiveHeight: true,
		responsive: [
		{
		  breakpoint: 1024,
		  settings: {
		  	dots: true,
		    slidesToShow: 3,
		    slidesToScroll: 3,
		    infinite: true
		  }
		},
		{
		  breakpoint: 767,
		  settings: {
		    slidesToShow: 2,
		    slidesToScroll: 2
		  }
		},
		{
		  breakpoint: 480,
		  settings: {
		    dots: false,
		    slidesToShow: 1,
		    slidesToScroll: 1
		  }
		}
		]
	});
	jQuery('.slider-sec .slick-slider').slick({
		dots: true,
		arrows: true,
		slidesToShow: 1,
		centerMode: true,
		centerPadding: '30%',
		responsive: [
		{
			breakpoint: 768,
			settings: {
				arrows: false,
				centerMode: true,
				centerPadding: '20%',
				slidesToShow: 1
			}
		},
		{
			breakpoint: 480,
			settings: {
				arrows: false,
				centerMode: true,
				centerPadding: '20%',
				slidesToShow: 1
			}
		}
		]
	});
	jQuery('.product-slider').slick({
		slidesToShow: 1,
		slidesToScroll: 1,
		arrows: false,
		fade: true,
		centerPadding: '0',
		asNavFor: '.pagg-slider'
	});
	// jQuery('.pagg-slider').slick({
	// 	slidesToShow: 8, // Muestra 4 imágenes a la vez
	// 	slidesToScroll: 1, // Desplaza de una en una
	// 	infinite: true, // Desactiva el loop infinito
	// 	arrows: false, // Muestra flechas de navegación
	// 	centerPadding: '0',
	// 	asNavFor: '.product-slider',
	// 	focusOnSelect: true, // Permite seleccionar imágenes
	// 	variableWidth: false 
	// });
	jQuery('.pagg-slider img').on('click', function () {
		// Quitar la clase "selected" de todas las imágenes
		jQuery('.pagg-slider img').removeClass('image-selected');
	
		// Agregar la clase "selected" a la imagen actual
		jQuery(this).addClass('image-selected');
	});	
}

// IsoTop init
function initIsoTop() {
	// Isotope init
	var isotopeHolder = jQuery('#masonry-container'),
		win = jQuery(window);
	jQuery('#masonry-container').isotope({
		itemSelector: '.product-block',
		transitionDuration: '0.6s'
	});
	jQuery('.filter-list a').click(function(e){
		e.preventDefault();
		
		jQuery('.filter-list li').removeClass('active');
		jQuery(this).parent('li').addClass('active');
		var selector = jQuery(this).attr('data-filter');
		isotopeHolder.isotope({ filter: selector });
	});
}

// sticky header init
function initbackTop() {
	var jQuerybackToTop = jQuery("#back-top");
	jQuery(window).on('scroll', function() {
		if (jQuery(this).scrollTop() > 100) {
			jQuerybackToTop.addClass('active');
		} else {
			jQuerybackToTop.removeClass('active');
		}
	});
	jQuerybackToTop.on('click', function(e) {
		jQuery("html, body").animate({scrollTop: 0}, 500);
	});
}

// PreLoader init
function initPreLoader() {
	jQuery('#pre-loader').delay(400).fadeOut();
}

// style changer
function initStyleChanger() {
	var element = jQuery('#style-changer');

	if(element) {
		$.ajax({
			url: element.attr('data-src'),
			type: 'get',
			dataType: 'text',
			success: function(data) {
				var newContent = jQuery('<div>', {
					html: data
				});

				newContent.appendTo(element);
				jQuery(".changer-opener").click(function(event){
					event.preventDefault();
					jQuery("body").toggleClass("changer-active");
				});
				
				var sheet,
					darkSheet,
					sheetName,
					darkSheetName = 'dark',
					sheetAdded = false,
					darkStylesAdded = false;

				jQuery('.list-color li').each(function() {
					var item = jQuery(this),
						link = item.find('a').eq(0);

					link.on('click', function(e) {
						e.preventDefault();
						sheetName = item.attr('class');

						if(!sheetAdded) {
							sheet = jQuery('<link>').attr('rel','stylesheet')
										.attr('type','text/css')
										.attr('href', 'css/color/' + sheetName + '.css')
										.appendTo('head');

							sheetAdded = true;
						} else {
							sheet.attr('href', 'css/color/' + sheetName + '.css');
						}
					});
				});
			}
		});
	}
}

// sticky header init
function initStickyHeader() {
	var win = jQuery(window),
		stickyClass = 'sticky';

	jQuery('#header.sticky-header').each(function() {
		var header = jQuery(this);
		var headerOffset = header.offset().top || 0;
		var flag = true;

		function scrollHandler() {
			if (win.scrollTop() > headerOffset) {
				if (flag){
					flag = false;
					header.addClass(stickyClass);
				}
			} else {
				if (!flag) {
					flag = true;
					header.removeClass(stickyClass);
				}
			}

			ResponsiveHelper.addRange({
				'..767': {
					on: function() {
						header.removeClass(stickyClass);
					}
				}
			});
		}

		scrollHandler();
		win.on('scroll resize orientationchange', scrollHandler);
	});
}

// Counter init
function initCounter() {
	jQuery('.counter').counterUp({
		delay: 10,
		time: 2000
	});
}

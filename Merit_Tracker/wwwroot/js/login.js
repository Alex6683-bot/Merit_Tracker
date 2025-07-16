var form = document.querySelector('.login-form');
var submit_button = document.querySelector('.login-submit');

var userInput = document.querySelector('#usernameInput');
var passwordInput = document.querySelector('#passwordInput');
var inputElements = form.querySelectorAll('.form-control');

form.addEventListener('submit', function (event) {

    event.preventDefault();

    addButtonLoad();

    if (form.checkValidity()) {
        userInput.value = userInput.value.trim();
        passwordInput.value = passwordInput.value.trim();

        let data = new FormData(form);
        fetch('', {
            method: 'post',
            body: data
        }).then(response => {

            removeButtonLoad();

            if (response.status != 200) {
                addFormInvalid();
                return;
            }
            response.json().then(data => {
                window.location.href = data.url;
            });
        });

    }
    else {
        addFormInvalid();
        removeButtonLoad();
    }

}, false);

inputElements.forEach((input) => {
    input.addEventListener('input', () => {
        if (form.classList.contains('was-validated')) form.classList.remove('was-validated');
        removeFormInvalid();
    })
})


function removeButtonLoad() {
    let button_content = submit_button.querySelector('.content');
    let spinner_container = submit_button.querySelector('.spinner-container');

    button_content.style.display = 'inline-block';
    spinner_container.style.display = 'none';
}

function addButtonLoad() {
    let button_content = submit_button.querySelector('.content');
    let spinner_container = submit_button.querySelector('.spinner-container');


    button_content.style.display = 'none';
    spinner_container.style.display = 'inline-block';
}

function addFormInvalid() {
    inputElements.forEach((input) => {
        input.classList.add('is-invalid');
    })
}

function removeFormInvalid() {
    inputElements.forEach((input) => {
        if (input.classList.contains('is-invalid')) input.classList.remove('is-invalid');
    })
}


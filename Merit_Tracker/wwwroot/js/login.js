var form = document.querySelector('.login-form');
var submit_button = document.querySelector('.login-submit');

var inputElements = form.querySelectorAll('.form-control');

form.addEventListener('submit', function (event) {

    event.preventDefault();
    //validate_inputs();

    add_button_load();

    if (form.checkValidity()) {
        let data = new FormData(form);
        fetch('', {
            method: 'post',
            body: data
        }).then(response => {

            remove_button_load();

            if (response.status != 200) {
                make_form_invalid();
                return;
            }
            else {
                window.location.href = response.url;
            }
        });

    }
    else {
        make_form_invalid();
        remove_button_load();
    }

}, false);

inputElements.forEach((input) => {
    input.addEventListener('input', () => {
        if (form.classList.contains('was-validated')) form.classList.remove('was-validated');
        remove_form_invalid();
    })
})


function remove_button_load() {
    let button_content = submit_button.querySelector('.content');
    let spinner_container = submit_button.querySelector('.spinner-container');

    button_content.style.display = 'inline-block';
    spinner_container.style.display = 'none';
}

function add_button_load() {
    let button_content = submit_button.querySelector('.content');
    let spinner_container = submit_button.querySelector('.spinner-container');


    button_content.style.display = 'none';
    spinner_container.style.display = 'inline-block';
}

function make_form_invalid() {
    inputElements.forEach((input) => {
        input.classList.add('is-invalid');
    })
}

function remove_form_invalid() {
    inputElements.forEach((input) => {
        if (input.classList.contains('is-invalid')) input.classList.remove('is-invalid');
    })
}

function validate_inputs() {
    inputElements.forEach((element) => {
        element.value = element.value.trim();
    })
}

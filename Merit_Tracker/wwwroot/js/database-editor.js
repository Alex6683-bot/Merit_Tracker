const addButton = document.querySelector('.add-button');
const editButton = document.querySelector('.edit-button');
const deleteButton = document.querySelector('.delete-button');

bindRadioListeners();
updateButtonState();

function bindRadioListeners() {
    const radioCheckBoxes = document.querySelectorAll('.merit-view-radio');
    radioCheckBoxes.forEach((radioCheckBox) => {
        radioCheckBox.addEventListener('change', updateButtonState);
    });
}


// Disable Edit & Delete buttons if radio button is not checked
function updateButtonState() {
    const radioCheckBoxes = document.querySelectorAll('.merit-view-radio');
    const anyChecked = Array.from(radioCheckBoxes).some(rb => rb.checked);
    editButton.disabled = !anyChecked;
    deleteButton.disabled = !anyChecked;
}

const meritRecordModal = document.querySelector('.merit-record-modal');
const meritForm = document.querySelector('.merit-form');
const modalTitle = document.querySelector('.modal-title');

// Modifying the same modal element based on the operation being done
function changeModalState(title, dataMode) {

    meritForm.reset();
    modalTitle.innerHTML = title;
    meritForm.setAttribute('data-mode', dataMode);

    // Fill input fields with the selected merit record if the modal is on edit state
    if (dataMode == 'edit') {
        const studentNameInput = document.querySelector('#studentName');
        const meritValueInput = document.querySelector('#meritValue');
        const meritHousePointsInput = document.querySelector('#housePoints');
        const meritIDInput = document.querySelector('#meritID');

        let selectedRadio = document.querySelector('input[name="merit-select-radio"]:checked');
        const meritRecord = selectedRadio.closest('.merit-view'); // Get merit view/record from the selected radio

        studentNameInput.value = meritRecord.getAttribute('data-student-name');
        meritValueInput.value = meritRecord.getAttribute('data-value');
        meritHousePointsInput.value = meritRecord.getAttribute('data-house-points');
        meritIDInput.value = meritRecord.getAttribute('data-merit-id');
    }
}


addButton.addEventListener('click', () => changeModalState("Add Merit Record", "add"));
editButton.addEventListener('click', () => changeModalState("Edit Merit Record", "edit"));

// Feedback Modal
function showFeedbackModal(isSuccess, message) {
    const feedbackModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('feedbackModal'));
    const header = document.getElementById('feedbackModalHeader');
    const body = document.getElementById('feedbackModalBody');

    // Reset previous states
    header.classList.remove('feedback-success', 'feedback-failure');

    if (isSuccess) {
        header.classList.add('feedback-success');
        document.getElementById('feedbackModalLabel').textContent = "Success";
    } else {
        header.classList.add('feedback-failure');
        document.getElementById('feedbackModalLabel').textContent = "Failed";
    }

    body.textContent = message;
    feedbackModal.show();
}

const meritModal = document.getElementById('merit-record-modal');
const meritModalInstance = bootstrap.Modal.getOrCreateInstance(meritModal);

// AJAX request for add merit
meritForm.addEventListener('submit', function (event) {

    // Validate the form
    if (!meritForm.checkValidity()) {
        event.preventDefault();
        event.stopPropagation();
        meritForm.classList.add('was-validated');
        return;
    }
    meritForm.classList.add('was-validated');

    event.preventDefault();
    let formData = new FormData(meritForm);

    // Request to add merit handler
    if (meritForm.getAttribute('data-mode') == 'add') {
        fetch('?handler=AddMerit', {
            method: 'post',
            body: formData
        }).then(response => {
            if (response.ok) {
                showFeedbackModal(true, "Merit registered successfully");
                meritForm.classList.remove('was-validated');
                return response.text();
            }
            showFeedbackModal(false, "Failed to register merit");

        }).then(responseHtml => {
            const meritList = document.querySelector('.merit-list');
            meritList.innerHTML = responseHtml;
            bindRadioListeners();
            updateButtonState();
        });
    }
    // Resquest to edit merit handler
    else if (meritForm.getAttribute('data-mode') == 'edit') {
        fetch('?handler=EditMerit', {
            method: 'post',
            body: formData
        }).then(response => {
            if (response.ok) {
                showFeedbackModal(true, "Merit edited successfully");
                meritForm.classList.remove('was-validated');
                return response.text();
            }
            showFeedbackModal(false, "Failed to edit merit");

        }).then(responseHtml => {
            const meritList = document.querySelector('.merit-list');
            meritList.innerHTML = responseHtml;
            bindRadioListeners();
            updateButtonState();
        });
    }

    
});


// Handle Delete Button
deleteButton.addEventListener('click', () => {

})








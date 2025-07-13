const radioCheckBoxes = document.querySelectorAll('.merit-view-radio');

const addButton = document.querySelector('.add-button');
const editButton = document.querySelector('.edit-button');
const deleteButton = document.querySelector('.delete-button');

addButton.disabled = true;
editButton.disabled = true;
deleteButton.disabled = true;


radioCheckBoxes.forEach((radioCheckBox) => {
    radioCheckBox.addEventListener('change', update_buttons_state);
});


// Disable CRUD buttons if radio button is not checked
function update_buttons_state() {
    const anyChecked = Array.from(radioCheckBoxes).some(rb => rb.checked);
    addButton.disabled = !anyChecked;
    editButton.disabled = !anyChecked;
    deleteButton.disabled = !anyChecked;
}




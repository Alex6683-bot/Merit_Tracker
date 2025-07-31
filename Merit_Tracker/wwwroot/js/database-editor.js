const addButton = document.querySelector('.add-button');
const editButton = document.querySelector('.edit-button');
const deleteButton = document.querySelector('.delete-button');
const filterButton = document.querySelector('.filter-button');


const filterStudentName = document.getElementById('filterStudentName');
const filterIssuerName = document.getElementById('filterIssuerName');
const filterMeritValue = document.getElementById('filterMeritValue');
const filterStartDate = document.getElementById('filterStartDate');
const filterEndDate = document.getElementById('filterEndDate');
const filterMeritYearLevel = document.getElementById('filterYearLevel');

const filterSubmitButton = document.querySelector('.merit-modal-filter-button');

const filterForm = document.querySelector('.filter-form');

let urlQueryParams = new URLSearchParams(window.location.search); // For filtering

bindRadioListeners();
updateControlButtonState();

// Binding listeners to each radio boxes
function bindRadioListeners() {
    const radioCheckBoxes = document.querySelectorAll('.merit-view-radio');
    radioCheckBoxes.forEach((radioCheckBox) => {
        radioCheckBox.addEventListener('change', updateControlButtonState);
    });
}


// Disable Edit & Delete buttons if radio button is not checked
function updateControlButtonState() {
    const radioCheckBoxes = document.querySelectorAll('.merit-view-radio');
    const anyChecked = Array.from(radioCheckBoxes).some(rb => rb.checked);

    if (editButton) editButton.disabled = !anyChecked;
    if (deleteButton) deleteButton.disabled = !anyChecked;
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
        const meritYearLevelInput = document.querySelector('#meritYearLevel');

        let selectedRadio = document.querySelector('input[name="merit-select-radio"]:checked');
        const meritRecord = selectedRadio.closest('.merit-view'); // Get merit view/record from the selected radio

        studentNameInput.value = meritRecord.getAttribute('data-student-name');
        meritValueInput.value = meritRecord.getAttribute('data-value');
        meritHousePointsInput.value = meritRecord.getAttribute('data-house-points');
        meritIDInput.value = meritRecord.getAttribute('data-merit-id');
        meritYearLevelInput.value = meritRecord.getAttribute('data-merit-year-level');
    }
}


if (addButton) addButton.addEventListener('click', () => changeModalState("Add Merit Record", "add"));
if (editButton) editButton.addEventListener('click', () => changeModalState("Edit Merit Record", "edit"));

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

// AJAX request for add & edit merit
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
        fetch('?handler=AddMerit&' + urlQueryParams.toString(), {
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
            if (responseHtml) {
                const meritList = document.querySelector('.merit-list');
                meritList.innerHTML = responseHtml;
                bindRadioListeners();
                updateControlButtonState();
                updateFilterInputs();
            }
        });
    }
    // Resquest to edit merit handler
    else if (meritForm.getAttribute('data-mode') == 'edit') {
        fetch('?handler=EditMerit&' + urlQueryParams.toString(), {
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
            if (responseHtml) {
                const meritList = document.querySelector('.merit-list');
                meritList.innerHTML = responseHtml;
                bindRadioListeners();
                updateControlButtonState();
                updateFilterInputs();

            }
        });
    }

});

const deleteModal = new bootstrap.Modal(document.getElementById('delete-confirmation-modal'));

if (deleteButton) {
    deleteButton.addEventListener('click', () => {
        deleteModal.show();

    });
}

const confirmDeleteButton = document.querySelector('#confirmDelete');

// AJAX request for delete merit
confirmDeleteButton.addEventListener('click', () => {
    let selectedRadio = document.querySelector('input[name="merit-select-radio"]:checked');
    const meritRecord = selectedRadio.closest('.merit-view'); // Get merit view/record from the selected radio
    const meritId = meritRecord.getAttribute('data-merit-id');

    const formData = new FormData();
    formData.append('MeritID', meritId);

    fetch('?handler=DeleteMerit&' + urlQueryParams.toString(), {
        method: 'post',
        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() }, // Fix anti forgery issues
        body: formData
    }).then(response => {
        deleteModal.hide();
        if (response.ok) {
            showFeedbackModal(true, "Merit deleted successfully");
            return response.text();
        }
        showFeedbackModal(false, "Failed to delete merit");
    }).then(responseHtml => {
        if (responseHtml) {
            const meritList = document.querySelector('.merit-list');
            meritList.innerHTML = responseHtml;
            bindRadioListeners();
            updateControlButtonState();
            updateFilterInputs();
        }
    })
});




function removeParams() {
    urlQueryParams.delete("studentName");
    urlQueryParams.delete("issuerName");
    urlQueryParams.delete("value");
    urlQueryParams.delete("startDate");
    urlQueryParams.delete("endDate");
    urlQueryParams.delete("yearLevel");

    history.replaceState(null, '', '?' + urlQueryParams.toString()); // updating the url route values

}

// Show filter modal on click
const filterModal = new bootstrap.Modal(document.getElementById('merit-filter-modal'));
filterButton.addEventListener('click', () => {
    filterModal.show();
});


function updateFilterInputs() {
    let hasAnyFilters = !(filterStudentName.value.trim() == "" &&
        filterIssuerName.value.trim() == "" &&
        parseInt(filterMeritValue.value, 10) == 0 &&
        filterStartDate.valueAsDate == null &&
        filterEndDate.valueAsDate == null &&
        filterMeritYearLevel.value.trim() == ""
    );

    // Validate Dates
    const hasValidDateRange =
        (filterStartDate.valueAsDate && !filterEndDate.valueAsDate) ||
        (!filterStartDate.valueAsDate && filterEndDate.valueAsDate) ||
        (!filterStartDate.valueAsDate && !filterEndDate.valueAsDate && hasAnyFilters) || 
        (filterStartDate.valueAsDate && filterEndDate.valueAsDate && filterStartDate.valueAsDate <= filterEndDate.valueAsDate);

    filterSubmitButton.disabled = !(hasAnyFilters && hasValidDateRange);
}

updateFilterInputs();

// Validation
filterForm.querySelectorAll('input, select').forEach(input => {
    input.addEventListener('change', () => {
        updateFilterInputs();
    })
})

// AJAX request for filter
filterForm.addEventListener('submit', function (event) {
    event.preventDefault();

    // Filter object
    let filterData = {
        StudentNameFilter: filterStudentName.value.trim(),
        IssuerNameFilter: filterIssuerName.value.trim(),
        YearLevelFilter: filterMeritYearLevel.value.trim(),
        MeritValueFilter: parseInt(filterMeritValue.value, 0),
        MeritStartDateFilter: filterStartDate.valueAsDate,
        MeritEndDateFilter: filterEndDate.valueAsDate
    };

    removeParams();


    if (filterData.StudentNameFilter) urlQueryParams.append("studentName", filterData.StudentNameFilter);
    if (filterData.IssuerNameFilter) urlQueryParams.append("issuerName", filterData.IssuerNameFilter);
    if (filterData.MeritValueFilter !== 0) urlQueryParams.append("value", filterData.MeritValueFilter);
    if (filterData.MeritStartDateFilter) urlQueryParams.append("startDate", filterStartDate.value);
    if (filterData.MeritEndDateFilter) urlQueryParams.append("endDate", filterEndDate.value);
    if (filterData.YearLevelFilter) urlQueryParams.append("yearLevel", filterData.YearLevelFilter);
                              

    fetch('?handler=FilterMerit&' + urlQueryParams.toString(), {
        method: 'post',
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val(),  // Fix anti forgery issues
            'Content-Type': 'application/json'
        },
    }).then(response => {
        if (response.ok) {
            history.replaceState(null, '', '?' + urlQueryParams.toString()); // updating the url route values
            return response.text();
        }
    }).then(responseHtml => {
        if (responseHtml) {
            filterModal.hide();
            const meritList = document.querySelector('.merit-list');
            meritList.innerHTML = responseHtml;
            bindRadioListeners();
            updateControlButtonState();
        }
    })
})

function removeFilter() {
    fetch('?handler=RemoveFilter', {
        method: 'post',
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val(),  // Fix anti forgery issues
            'Content-Type': 'application/json'
        },
        body: null
    }).then(response => {
        if (response.ok) {
            return response.text();
        }
    }).then(responseHtml => {
        if (responseHtml) {
            filterForm.reset();
            updateFilterInputs();
            removeParams();
            filterModal.hide();
            const meritList = document.querySelector('.merit-list');
            meritList.innerHTML = responseHtml;
            bindRadioListeners();
            updateControlButtonState();
        }
    })
};






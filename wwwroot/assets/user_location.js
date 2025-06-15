var locationData = []

const countrySelect = document.getElementById('country');
const stateSelect = document.getElementById('state');
const citySelect = document.getElementById('city');


function populateStates() {
    const countrySelect = document.getElementById('country');
    const stateSelect = document.getElementById('state');
    const citySelect = document.getElementById('city');
    const selectedCountry = countrySelect.value;

    // Clear previous options
    stateSelect.innerHTML = '<option value="" selected>Select State/Province</option>';
    citySelect.innerHTML = '<option value="" selected>Select City</option>';

    for (const country of locationData) {
        if (country.name != selectedCountry || !country.states) continue;

        country.states.forEach(state => {
            const option = document.createElement('option');
            option.value = state.name;
            option.text = state.name;
            stateSelect.appendChild(option);
        });

        break; // Exit loop after finding the selected country
    }
}

function populateCities() {
    const countrySelect = document.getElementById('country');
    const stateSelect = document.getElementById('state');
    const citySelect = document.getElementById('city');
    const selectedCountry = countrySelect.value;
    const selectedState = stateSelect.value;

    // Clear previous options
    citySelect.innerHTML = '<option value="" selected>Select City</option>';

    const countryData = locationData.find(country => country.name == selectedCountry);
    if (!countryData || !countryData.states) return;

    const stateData = countryData.states.find(state => state.name == selectedState);
    if (!stateData || !stateData.cities) return;

    stateData.cities.forEach(city => {
        const option = document.createElement('option');
        option.value = city.name;
        option.textContent = city.name;
        citySelect.appendChild(option);
    });
}

document.addEventListener('DOMContentLoaded', async function () {

    const countrySelect = document.getElementById('country');
    const stateSelect = document.getElementById('state');

    const response = await fetch('/assets/countries_states_cities.json')
    if (!response.ok) {
        console.error('Error loading location data:', response.statusText)
        return;
    }
    locationData = await response.json();

    if (countrySelect && stateSelect) {
        for (const country of locationData) {
            const option = document.createElement('option');
            option.value = country.name;
            option.textContent = country.name;
            countrySelect.appendChild(option);
        }

        countrySelect.addEventListener('change', populateStates);
        stateSelect.addEventListener('change', populateCities);
    }
});
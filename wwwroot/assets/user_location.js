const locationCache = new Map();
const locationRequests = new Map();

function populateDropDown(element, values) {
    values.forEach(v => {
        const option = document.createElement('option');
        option.value = v;
        option.text = v;
        element.appendChild(option);
    });
}

async function getLocation(level, country = '', state = '') {
    const key = `${level}:${country.trim().toLowerCase()}:${state.trim().toLowerCase()}`;
    if (locationCache.has(key)) {
        return locationCache.get(key);
    }

    if (locationRequests.has(key)) {
        return locationRequests.get(key);
    }

    const query = new URLSearchParams();
    if (country) query.set('Country', country);
    if (state) query.set('State', state);
    const request = fetch(`/location?${query}`, { signal: locationController.signal })
        .then(response => {
            if (!response.ok) throw new Error(`Location request failed: ${response.status}`);
            return response.json();
        })
        .then(values => {
            const normalized = [...new Set(values.map(value => value.trim()).filter(Boolean))];
            locationCache.set(key, normalized);
            return normalized;
        })
        .finally(() => locationRequests.delete(key));
    locationRequests.set(key, request);
    return request;
}

let locationController = new AbortController();

async function populateStates() {
    const selectedCountry = document.getElementById('country').value;
    locationController.abort();
    locationController = new AbortController();
    try {
        const values = await getLocation('states', selectedCountry);
        let cityEle = document.getElementById('city')
        cityEle.disabled = true
        cityEle.innerHTML = '<option value="" selected disabled>Select City</option>'

        var ele = document.getElementById('state')
        ele.disabled = false
        ele.innerHTML = '<option value="" selected disabled>Select State/Province</option>'
        populateDropDown(document.getElementById('state'), values)
    } catch (error) {
        if (error.name !== 'AbortError') console.error(error);
    }
}

async function populateCities() {
    const selectedCountry = document.getElementById('country').value;
    const selectedState = document.getElementById('state').value;

    try {
        const values = await getLocation('cities', selectedCountry, selectedState);
        const ele = document.getElementById('city')
        ele.disabled = false
        ele.innerHTML = '<option value="" selected disabled>Select City</option>'
        populateDropDown(ele, values)
    } catch (error) {
        if (error.name !== 'AbortError') console.error(error);
    }
}

async function load(country, state, city) {
    const countrySelect = document.getElementById('country');
    const stateSelect = document.getElementById('state');

    if (countrySelect && stateSelect) {

        try {
            const values = await getLocation('countries');
            populateDropDown(document.getElementById('country'), values)
        } catch (error) {
            console.error(error);
        }

        countrySelect.addEventListener('change', populateStates);
        stateSelect.addEventListener('change', populateCities);

        const citySelect = document.getElementById('city');

        if (countrySelect && country) {
            countrySelect.value = country;
            await populateStates()

            if (stateSelect && state) {
                stateSelect.value = state;
                await populateCities()

                if (citySelect && city) {
                    citySelect.value = city;
                }
            }
        }
    }
};


function selectCountry(country) {
    const countrySelect = document.getElementById('country');
    if (countrySelect && country) {
        countrySelect.value = country;
        countrySelect.dispatchEvent(new Event('change'));
    }
}

function selectState(state) {
    const stateSelect = document.getElementById('state');
    if (stateSelect && state) {
        stateSelect.value = state;
        stateSelect.dispatchEvent(new Event('change'));
    }
}


function selectCity(city) {
    const citySelect = document.getElementById('city');
    if (citySelect && city) {
        citySelect.value = city;
    }
}
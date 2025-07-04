﻿
function populateDropDown(element, values) {
    values.forEach(v => {
        const option = document.createElement('option');
        option.value = v;
        option.text = v;
        element.appendChild(option);
    });
}

async function handleLocationResponse(res) {
    let values = []
    const data = await res.json()
    data.forEach(line => {
        values.push(line.trim())
    });
    return values;
}


async function populateStates() {
    const selectedCountry = document.getElementById('country').value;

    const res = await fetch('/location?' + new URLSearchParams({
        Country: selectedCountry
    }),
        {
            method: 'POST'
        })

    if (res.ok) {
        const values = await handleLocationResponse(res);

        let cityEle = document.getElementById('city')
        cityEle.disabled = true
        cityEle.innerHTML = '<option value="" selected disabled>Select City</option>'

        var ele = document.getElementById('state')
        ele.disabled = false
        ele.innerHTML = '<option value="" selected disabled>Select State/Province</option>'
        populateDropDown(document.getElementById('state'), values)
    }

}

async function populateCities() {
    const selectedCountry = document.getElementById('country').value;
    const selectedState = document.getElementById('state').value;

    const res = await fetch('/location?' + new URLSearchParams({
        Country: selectedCountry, State: selectedState
    }),
        {
            method: 'POST'
        })
    if (res.ok) {
        const values = await handleLocationResponse(res);
        const ele = document.getElementById('city')
        ele.disabled = false
        ele.innerHTML = '<option value="" selected disabled>Select City</option>'
        populateDropDown(ele, values)
    }

}

async function load(country, state, city) {
    const countrySelect = document.getElementById('country');
    const stateSelect = document.getElementById('state');

    if (countrySelect && stateSelect) {

        const res = await fetch('/location',
            {
                method: 'POST'
            })

        if (res.ok) {
            const values = await handleLocationResponse(res);
            populateDropDown(document.getElementById('country'), values)
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
(function () {
    "use strict";

    const conversionRates = [
        { from: "km", to: "m", rate: 1000 },
        { from: "km", to: "cm", rate: 1000 * 100 },

        { from: "km", to: "mi", rate: 1000 / 1609.344 },
        { from: "km", to: "yd", rate: 1000 / 0.9144 },
        { from: "km", to: "ft", rate: 1000 / 0.3048 },
        { from: "km", to: "in", rate: 1000 / 0.0254 },

        { from: "m", to: "km", rate: 1 / 1000 },
        { from: "m", to: "cm", rate: 100 },

        { from: "m", to: "mi", rate: 1 / 1609.344 },
        { from: "m", to: "yd", rate: 1 / 0.9144 },
        { from: "m", to: "ft", rate: 1 / 0.3048 },
        { from: "m", to: "in", rate: 1 / 0.0254 },

        { from: "cm", to: "km", rate: 1 / 100 / 1000 },
        { from: "cm", to: "m", rate: 1 / 100 },

        { from: "cm", to: "mi", rate: 1 / 1609.344 / 100 },
        { from: "cm", to: "yd", rate: 1 / 91.44 },
        { from: "cm", to: "ft", rate: 1 / 30.48 },
        { from: "cm", to: "in", rate: 1 / 2.54 },

        { from: "mi", to: "yd", rate: 1760 },
        { from: "mi", to: "ft", rate: 1760 * 3 },
        { from: "mi", to: "in", rate: 1760 * 3 * 4 },

        { from: "mi", to: "km", rate: 1609.344 / 1000 },
        { from: "mi", to: "m", rate: 1609.344 },
        { from: "mi", to: "cm", rate: 1609.344 * 100 },

        { from: "yd", to: "mi", rate: 1 / 1760 },
        { from: "yd", to: "ft", rate: 3 },
        { from: "yd", to: "in", rate: 3 * 4 },

        { from: "yd", to: "km", rate: 0.9144 / 1000 },
        { from: "yd", to: "m", rate: 0.9144 },
        { from: "yd", to: "cm", rate: 91.44 },

        { from: "ft", to: "mi", rate: 1 / 3 / 1760 },
        { from: "ft", to: "yd", rate: 1 / 3 },
        { from: "ft", to: "in", rate: 4 },

        { from: "ft", to: "km", rate: 0.3048 / 1000 },
        { from: "ft", to: "m", rate: 0.3048 },
        { from: "ft", to: "cm", rate: 30.48 },

        { from: "in", to: "mi", rate: 1 / 4 / 3 / 1760 },
        { from: "in", to: "yd", rate: 1 / 4 / 3 },
        { from: "in", to: "ft", rate: 1 / 4 },

        { from: "in", to: "km", rate: 0.0254 / 1000 },
        { from: "in", to: "m", rate: 0.0254 },
        { from: "in", to: "cm", rate: 2.54 }
    ];

    const scaleValue = document.getElementById("scale-value");
    scaleValue.onchange = convertFirstValueToSecondValue;

    const firstValue = document.getElementById("first-value");
    firstValue.oninput = convertFirstValueToSecondValue;

    const firstUnit = document.getElementById("first-unit");
    firstUnit.onchange = convertFirstValueToSecondValue;       

    const secondValue = document.getElementById("second-value");
    secondValue.oninput = convertSecondValueToFirstValue;

    const secondUnit = document.getElementById("second-unit");        
    secondUnit.onchange = convertFirstValueToSecondValue;

    function convertFirstValueToSecondValue() {
        const scale = Number.parseFloat(scaleValue.value);
        const fromValue = Number.parseFloat(firstValue.value) * scale;
        const fromUnit = firstUnit.value;
        const toUnit = secondUnit.value;        
        secondValue.value = convert(fromValue, fromUnit, toUnit);
    }

    function convertSecondValueToFirstValue() {
        const scale = Number.parseFloat(scaleValue.value);
        const fromValue = Number.parseFloat(secondValue.value) / scale;
        const fromUnit = secondUnit.value;
        const toUnit = firstUnit.value;
        firstValue.value = convert(fromValue, fromUnit, toUnit);
    }

    function convert(fromValue, fromUnit, toUnit) {
        if (isNaN(fromValue) || (fromValue < 0)) {
            return "";
        }

        const rate = getConversionRate(fromUnit, toUnit);
        const result = fromValue * rate;

        if (Number.isInteger(result)) {
            return result.toString();
        } else {
            return result.toFixed(3);
        }
    }

    function getConversionRate(from, to) {
        const conversion = conversionRates.find(conversion => (conversion.from === from) && (conversion.to === to));
        return conversion !== undefined ? conversion.rate : 1;
    }
}());
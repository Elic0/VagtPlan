window.vagtPlanDateInput = (function () {
    const instances = new WeakMap();

    function getInstance(element) {
        return instances.get(element);
    }

    function toIsoDate(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${year}-${month}-${day}`;
    }

    function isWeekend(date) {
        const day = date.getDay();
        return day === 0 || day === 6;
    }

    return {
        init: function (element, dotnetRef, initialValue, disabled) {
            if (!element || typeof flatpickr === "undefined") {
                return;
            }

            const existing = getInstance(element);
            if (existing) {
                existing.destroy();
            }

            const fp = flatpickr(element, {
                locale: flatpickr.l10ns.da,
                dateFormat: "d/m/Y",
                allowInput: true,
                disableMobile: true,
                monthSelectorType: "static",
                animate: true,
                defaultDate: initialValue || undefined,
                disable: [isWeekend],
                onReady: function (_selectedDates, _dateStr, instance) {
                    instance.calendarContainer.classList.add("vp-flatpickr");
                },
                onChange: function (selectedDates, _dateStr, instance) {
                    if (selectedDates.length > 0) {
                        if (isWeekend(selectedDates[0])) {
                            instance.clear();
                            dotnetRef.invokeMethodAsync("OnDateCleared");
                            return;
                        }

                        dotnetRef.invokeMethodAsync("OnDateSelected", toIsoDate(selectedDates[0]));
                    } else {
                        dotnetRef.invokeMethodAsync("OnDateCleared");
                    }
                }
            });

            instances.set(element, fp);

            if (disabled) {
                // flatpickr expects an array or function for "disable" — passing a boolean causes
                // a JS error (e.slice is not a function). Disable all dates via a function and
                // also disable the input element to prevent interaction.
                fp.set("disable", [function () { return true; }]);
                if (fp.input) {
                    fp.input.disabled = true;
                }
            } else {
                fp.set("disable", [isWeekend]);
                if (fp.input) {
                    fp.input.disabled = false;
                }
            }
        },

        setDate: function (element, isoDate) {
            const fp = getInstance(element);
            if (!fp) {
                return;
            }

            if (isoDate) {
                fp.setDate(isoDate, false);
            } else {
                fp.clear();
            }
        },

        setDisabled: function (element, disabled) {
            const fp = getInstance(element);
            if (!fp) {
                return;
            }

            // Do not pass a boolean to flatpickr's "disable" option. Use a function that
            // always returns true to disable all dates, and toggle the input's disabled state.
            if (disabled) {
                fp.set("disable", [function () { return true; }]);
                if (fp.input) {
                    fp.input.disabled = true;
                }
            } else {
                fp.set("disable", [isWeekend]);
                if (fp.input) {
                    fp.input.disabled = false;
                }
            }
        },

        destroy: function (element) {
            const fp = getInstance(element);
            if (fp) {
                fp.destroy();
                instances.delete(element);
            }
        }
    };
})();

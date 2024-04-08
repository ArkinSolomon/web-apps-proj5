/**
 * @enum Term
 */
const Term = {
    Spring: 0,
    Summer: 1,
    Fall: 2,
}

/**
 * @typedef {Object} PlannedCourse
 * @property {string} id
 * @property {number} year
 * @property {Term} term
 */

/**
 * @typedef {Object} Course
 * @property {string} id The course id (like CS-120)
 * @property {string} name The name of the course
 * @property {string} description
 * @property {number} credits
 */

/**
 * @typedef {Object} Plan
 * @property {int} id The plan's id.
 * @property {string} name The name of the plan.
 * @property {Object.<string, string>} majors The plan's majors.
 * @property {Object.<string, string>} minors The plan's minors.
 * @property {number} catalog The catalog year of the plan.
 */

/**
 * @typedef {Object} CatalogData
 * @property {Object} plan
 * @property {string} plan.student The student's name
 * @property {string} [plan.name] The name of the current plan
 * @property {string[]} plan.majors The current plan's majors
 * @property {string[]} plan.minors The current plan's majors
 * @property {number} plan.currYear The current year the student is in
 * @property {Term} plan.currTerm The current term the student is in
 * @property {number|null} plan.id The id of the currently loaded plan.
 * @property {number} plan.catYear The student's catalog year
 * @property {Object.<string, PlannedCourse>} plan.courses
 * @property {Object.<int, Plan>} plans The plans that the user has, indexed by id.
 * @property {Object} catalog
 * @property {number} catalog.year The year of this catalog (related to the current plan).
 * @property {Object.<string, Course>} catalog.courses All of the courses in the catalog.
 * @property {Object} catalog.accomplishments All of the accomplishments in the catalog.
 * @property {Object.<int, string>} catalog.accomplishments.majors
 * @property {Object.<int, string>} catalog.accomplishments.minors
 */

/**
 * @type {CatalogData}
 */
let catalogData;

/**
 * @typedef {Object} RequirementsData
 * @property {string[]} core
 * @property {string[]} electives
 * @property {string[]} cognates
 * @property {string[]} genEds
 */

/**
 * @typedef {('course_id'|'title'|'description'|'credits')} FilterCol
 */

/**
 * @type {RequirementsData}
 */
let requirementsData;

/**
 * @type {string|null}
 */
let waitForFilter = null;
const filterWorker = new Worker('/js/filterWorker.js');

/**
 * @param {string} id
 * @returns {Course|null}
 */
function findCourse(id) {
    return catalogData.catalog.courses[id] ?? null;
}

/**
 * @type {string}
 */
let initialMajors, initialMinors;

const matches = /Index\/([a-f0-9-]+)\/?$/i.exec(window.location.href);
let suffix = matches && matches[1] ? `/${matches[1]}` : '/';

function renderCourses() {
    $('.hours').text('Hours: 0')
    $('.term-block').find('p:gt(1)').remove();

    $(`.accordion-course`).removeClass('req-satisfied');

    for (const course of Object.values(catalogData.plan.courses)) {
        const {name: courseName, credits} = findCourse(course.id);
        const $termBlock = $(`#term-${course.year}-${course.term}`);
        $termBlock.append(`<p draggable="true" class="listed-course draggable-course" ondragstart="drag(event, '${course.id}');">${course.id} ${courseName}<button class="rm-course-button" onclick="removeCourse('${course.id}');">X</button></p>`);
        const $hours = $(`#term-${course.year}-${course.term}-hours`);
        const currentHours = parseInt($hours.text().replace('Hours: ', ''), 10);
        $hours.text(`Hours: ${currentHours + credits}`);
        for (const [categoryName] of Object.entries(requirementsData)) {
            const $course = $(`#accordion-course-${categoryName}-${course.id}`);
            if ($course) {
                $course.addClass('req-satisfied');
            }
        }
    }
}

async function loadData() {
    [catalogData, requirementsData] = await Promise.all([
        $.ajax('/Planner/Catalog' + suffix).promise(),
        $.ajax('/Planner/Requirements' + suffix).promise()
    ]);

    $('body')[0].classList.remove('loading');

    filterWorker.postMessage({
        initial_data: true,
        courses: Object.values(catalogData.catalog.courses)
    });

    filterWorker.onmessage = e => {
        console.log(e.data.courses.length);
        if (Object.hasOwn(e.data, 'filter') && e.data.filter !== waitForFilter) {
            return;
        }
        renderTable(e.data.courses);
    };
    
    populateAccordion();
    populateTable();

    if (catalogData.plan.id !== null) {
        let year = catalogData.plan.catYear;

        /**
         * @type {Term}
         */
        let term = Term.Fall;

        let newHtml = '';
        while (true) {
            newHtml += `<div id="term-${year}-${term}" class="term-block" ondragover="event.preventDefault();" ondrop="drop(event, ${year}, '${term}')"><p class="term">${termString(term)} ${year}</p><p class="hours" id="term-${year}-${term}-hours">Hours: 0</p></div>`;
            if (term === Term.Fall) {
                ++year;
                term = Term.Spring;
            } else if (term === Term.Spring) {
                term = Term.Summer;
            } else {
                term = Term.Fall;
            }

            if (year === catalogData.plan.catYear + 4 && term === Term.Fall) {
                break;
            }
        }

        const currentPlan = catalogData.plans[catalogData.plan.id];
        suggestedMajors = JSON.parse(JSON.stringify(currentPlan.majors));
        suggestedMinors = JSON.parse(JSON.stringify(currentPlan.minors));
        initialMajors = Object.keys(suggestedMajors).join();
        initialMinors = Object.keys(suggestedMinors).join();

        $('#term-grid').html(newHtml);

        renderCourses();

        $('#search').keyup(function () {
            populateTable($(this).val());
        });

        const $courseIdCol = $('#course-id-col');
        const $courseTitleCol = $('#course-title-col');
        const $courseDescCol = $('#course-desc-col');
        const $courseCreditsCol = $('#course-credits-col');

        const clearSortClass = () => {
            $courseIdCol.removeClass('sorted-by');
            $courseTitleCol.removeClass('sorted-by');
            $courseDescCol.removeClass('sorted-by');
            $courseCreditsCol.removeClass('sorted-by');
        }

        $courseIdCol.click(() => {
            clearSortClass();
            $courseIdCol.addClass('sorted-by');
            filterWorker.postMessage({
                sort_change: true,
                newSort: 'course_id'
            });
            populateTable();
        });

        $courseTitleCol.click(() => {
            clearSortClass();
            $courseTitleCol.addClass('sorted-by');
            filterWorker.postMessage({
                sort_change: true,
                newSort: 'title'
            });
            populateTable();
        });

        $courseDescCol.click(() => {
            clearSortClass();
            $courseDescCol.addClass('sorted-by');
            filterWorker.postMessage({
                sort_change: true,
                newSort: 'description'
            });
            populateTable();
        });

        $courseCreditsCol.click(() => {
            clearSortClass();
            $courseCreditsCol.addClass('sorted-by');
            filterWorker.postMessage({
                sort_change: true,
                newSort: 'credits'
            });
            populateTable();
        });
    } else {
        $('#no-plan').removeClass('hidden');
    }
    populateModal();
}

function populateAccordion() {
    const $accordion = $('#requirements-accordion');
    let content = '';

    if (catalogData.plan.id == null) {
        return;
    }

    for (const [categoryName, category] of Object.entries(requirementsData)) {
        content += `<h4>${categoryName[0].toUpperCase() + categoryName.slice(1)}</h4><div>`;
        for (const course of category) {
            const courseInfo = findCourse(course)
            if (!courseInfo) {
                continue;
            }
            content += `<p draggable="true" class="draggable-course accordion-course" id="accordion-course-${categoryName}-${courseInfo.id}" ondragstart="drag(event, '${courseInfo.id}');">${course} ${courseInfo.name}</p><hr class="accordion-separator">`;
        }
        content += '</div>';
    }

    $accordion.html(content);
    $accordion.accordion({
        active: null,
        header: 'h4',
        heightStyle: 'autoHeight',
        collapsible: true
    });
}

function populateTable(filter) {
    filter = filter ? filter.toLowerCase() : '';

    if (!catalogData.plan.id) {
        return;
    }
    
    waitForFilter = filter;
    filterWorker.postMessage(filter);
}

/**
 * @param {[Course]} courses
 */
function renderTable(courses) {
    let table = document.getElementById('course-finder-table-body');
    table.innerHTML = "";
    let content = "";
    for (let course of courses) {
        let newLine = `<tr class="draggable-course" draggable="true" ondragstart="drag(event, '${course.id}');"><td>${course.id}</td><td>${course.name}</td><td>${course.description}</td><td>${course.credits}</td></tr>`;
       content += newLine;
    }
    table.innerHTML = content;
}

/**
 * @type {Object.<string, string>}
 */
let suggestedMajors, suggestedMinors;

let modalEventListenersReady = false;

const genListItem = (id, name, type) => `<li class="acc-list-item" data-type="${type}" data-acc-id="${id}"><p>${name}</p><div class="remove-acc-button"><p>X</p></div></li>`;

function populateModal(resetSuggested = true) {
    $('#plan-modify-header span').text(catalogData.plan.id ? catalogData.plans[catalogData.plan.id].name : 'No Plan');

    const $planSelect = $('#plan-select');
    if (!Object.keys(catalogData.plans).length) {
        $planSelect.attr('disabled', true)
    } else {
        $planSelect.html('');
        for (const plan of Object.values(catalogData.plans)) {
            $planSelect.append(`<option value="${plan.id}" ${plan.id === catalogData.plan.id ? "selected" : ""}>${plan.name}</option>`);
        }
    }

    const $majorList = $('#major-list');
    const $minorList = $('#minor-list');
    const $addMajor = $('#major-add');
    const $addMinor = $('#minor-add');
    const $planNameUpdateField = $('#plan-name-update-field');

    $majorList.html('');
    $minorList.html('');
    $addMajor.html('');
    $addMinor.html('');

    if (catalogData.plan.id) {
        const currentPlan = catalogData.plans[catalogData.plan.id];
        $planNameUpdateField.attr('value', currentPlan.name);

        if (resetSuggested) {
            suggestedMajors = JSON.parse(JSON.stringify(currentPlan.majors));
            suggestedMinors = JSON.parse(JSON.stringify(currentPlan.minors));
        }

        Object.entries(suggestedMajors).forEach(([id, name]) => {
            $majorList.append(genListItem(id, name, 'major'));
        });

        Object.entries(suggestedMinors).forEach(([id, name]) => {
            $minorList.append(genListItem(id, name, 'minor'));
        });

        let hasRemainingMajor = false;
        Object.entries(catalogData.catalog.accomplishments.majors).forEach(([id, name]) => {
            if (Object.hasOwn(suggestedMajors, id)) {
                return;
            }
            hasRemainingMajor = true;
            $addMajor.append(`<option data-type="major" value="${id}">${name}</option>`);
        });

        let hasRemainingMinor = false;
        Object.entries(catalogData.catalog.accomplishments.minors).forEach(([id, name]) => {
            if (Object.hasOwn(suggestedMinors, id)) {
                return;
            }
            hasRemainingMinor = true;
            $addMinor.append(`<option data-type="minor" value="${id}">${name}</option>`);
        });

        if (!hasRemainingMajor) {
            $addMajor.append(`<option hidden selected disabled>&lt;No more majors&gt;</option>`);
            $addMajor.attr('disabled', true);
        } else {
            $addMajor.prepend(`<option hidden selected disabled>&lt;Add a major&gt;</option>`);
        }

        if (!hasRemainingMinor) {
            $addMinor.append(`<option hidden selected disabled>&lt;No more minors&gt;</option>`);
            $addMinor.attr('disabled', true);
        } else {
            $addMinor.prepend(`<option hidden selected disabled>&lt;Add a minor&gt;</option>`);
        }
    } else {
        $majorList.html('<p class="list-no-data">&lt;No data&gt;</p>');
        $minorList.html('<p class="list-no-data">&lt;No data&gt;</p>');

        $addMajor.attr('disabled', true);
        $addMajor.html('<option selected disabled hidden>&lt;Create a plan&gt;</option>');
        $addMinor.attr('disabled', true);
        $addMinor.html('<option selected disabled hidden>&lt;Create a plan&gt;</option>');
        $planNameUpdateField.attr('disabled', true);
    }

    const $loadPlanButton = $('#load-plan-button');

    const updateLoadPlan = () => $loadPlanButton.attr('disabled', $planSelect.val() === catalogData.plan.id);
    updateLoadPlan();

    $('.remove-acc-button').click(function () {
        const $removal = $(this).parent();

        const accId = $removal.data('acc-id');
        const accType = $removal.data('type');

        if (accType === 'major') {
            delete suggestedMajors[accId];
        } else {
            delete suggestedMinors[accId];
        }

        populateModal(false);
        determineIfSaveEnabled();
    });

    if (!modalEventListenersReady) {
        $planSelect.change(updateLoadPlan);

        $addMajor.change(() => {
            const id = $addMajor.val();
            const $opt = $addMajor.find('option:selected');
            const name = $opt.text();
            $majorList.append(genListItem(id, name, 'major'));
            $opt.remove();
            suggestedMajors[id] = name;
            determineIfSaveEnabled();
        });

        $addMinor.change(() => {
            const id = $addMinor.val();
            const $opt = $addMinor.find('option:selected');
            const name = $opt.text();
            $minorList.append(genListItem(id, name, 'minor'));
            $opt.remove();
            suggestedMinors[id] = name;
            determineIfSaveEnabled();
        });

        const $saveButton = $('#plan-modify-save');
        $saveButton.attr('disabled', !catalogData.plan.id);

        $saveButton.click(() => {
            const majorsStr = Object.keys(suggestedMajors).join();
            const minorsStr = Object.keys(suggestedMinors).join();

            const $updatePlanForm = $('#update-plan-form');
            $('#update-plan-form-name').attr('value', $planNameUpdateField.val());
            $('#update-plan-form-id').attr('value', catalogData.plan.id)
            $('#update-plan-form-majors').attr('value', majorsStr);
            $('#update-plan-form-minors').attr('value', minorsStr);

            $updatePlanForm.submit();
        });

        $planNameUpdateField.keyup(determineIfSaveEnabled)
    }
    determineIfSaveEnabled();
    modalEventListenersReady = true;
}

function determineIfSaveEnabled() {
    let isDisabled = false;

    if (catalogData.plan.id == null) {
        isDisabled = true;
    } else {

        const majorsStr = Object.keys(suggestedMajors).join();
        const minorsStr = Object.keys(suggestedMinors).join();

        const newPlanName = $('#plan-name-update-field').val();
        if (majorsStr === initialMajors && minorsStr === initialMinors && newPlanName === catalogData.plans[catalogData.plan.id].name) {
            isDisabled = true;
        }

        if (!isDisabled) {
            isDisabled = !newPlanName.length;
        }
    }

    $('#plan-modify-save').attr('disabled', isDisabled);
}

function drag(ev, id) {
    ev.dataTransfer.setData('course-id', id);
}

function removeCourse(courseId) {
    $.post('/Plan/RemoveCourse' + suffix, {
        'plan-id': catalogData.plan.id,
        'course-id': courseId,
    }).catch();
    delete catalogData.plan.courses[courseId];

    renderCourses();
}

function drop(ev, year, term) {
    ev.preventDefault();
    const courseId = ev.dataTransfer.getData('course-id');

    catalogData.plan.courses[courseId] = {
        id: courseId,
        year,
        term
    };

    $.post('/Plan/AddCourse' + suffix, {
        'plan-id': catalogData.plan.id,
        'course-id': courseId,
        year,
        term
    })
        .catch(() => {
            delete catalogData.plan.courses[courseId];
            renderCourses();
        });

    renderCourses();
}

/**
 * @param {Term} term
 */
function termString(term) {
    switch (term) {
        case Term.Spring:
            return "Spring";
        case Term.Summer:
            return "Summer";
        case Term.Fall:
            return "Fall";
    }
}
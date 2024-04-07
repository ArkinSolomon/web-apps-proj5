/**
 * @type {[Course]}
 */
let courses;

/**
 * @type {string|null}
 */
let lastFilter = null;

/**
 * @type {[[Course]]|null}
 */
let filterHistory = null;

/**
 * @type {FilterCol}
 */
let currentSort = 'course_id';

onmessage = (e) => {
    if (Object.hasOwn(e.data, "initial_data")) {
        courses = e.data.courses;
        courses.sort(courseSorter);
        filterHistory = [courses];
        postMessage({
            courses: courses,
        });
        return;
    } else if (Object.hasOwn(e.data, "sort_change")) {
        currentSort = e.data.newSort;
        filterHistory.toReversed().forEach((fh, i) => {
            fh.sort(courseSorter);
            if (!i) {
                postMessage({
                    courses: fh,
                    filter: lastFilter
                });
            }
        });
        return;
    }
    
    console.log(e.data);

    if (!filterHistory) {
        return;
    }

    /**
     * @type {string}
     */
    const filter = e.data;

    if (!lastFilter) {
        lastFilter = filter;
    }

    if (filter.length > lastFilter.length) {
        const newHistory = filterHistory[filterHistory.length - 1].filter(c => c.id.toLowerCase().includes(filter) || c.description.toLowerCase().includes(filter) || c.name.toLowerCase().includes(filter));
        filterHistory.push(newHistory);
        newHistory.sort(courseSorter);
    } else if (filterHistory.length > 1) {
        filterHistory.pop();
    }

    postMessage({courses: filterHistory[filterHistory.length - 1], filter});
    lastFilter = filter;
};

/**
 * @param {Course} c1
 * @param {Course} c2
 */
function courseSorter(c1, c2) {
    switch (currentSort) {
        case "course_id":
            return c1.id.localeCompare(c2.id);
        case "credits":
            return c2.credits - c1.credits;
        case "description":
            return c1.description.localeCompare(c2.description);
        case "title":
            return c1.name.localeCompare(c2.name);
    }
    return 0;
}
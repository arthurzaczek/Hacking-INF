import { Injectable } from '@angular/core';

import { Course } from './models';

@Injectable()
export class HackingService {
    getCourses(): Promise<Course[]> {
        return Promise.resolve([{ Title: "Test" }, { Title: "Hallo" }]);
    }
}
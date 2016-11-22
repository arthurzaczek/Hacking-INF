import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';
import { HomeComponent } from './home.component';
import { CoursesComponent } from './courses.component';
import { CourseDetailComponent } from './course-detail.component';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'courses', component: CoursesComponent },
            { path: 'course-detail/:name', component: CourseDetailComponent }
        ])
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        CoursesComponent,
        CourseDetailComponent,
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }

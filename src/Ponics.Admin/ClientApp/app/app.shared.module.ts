import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { AquaponicSystemComponent } from './components/aquaponicsystems/aquaponicsystem/aquaponicsystem.component';
import { AquaponicSystemsComponent } from './components/aquaponicsystems/aquaponicsystems.component';
import { AddAquaponicSystemComponent } from './components/aquaponicsystems/addaquaponicsystem/addaquaponicsystem.component';
import { AlertComponent } from './components/alert/alert.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        AquaponicSystemComponent,
        AquaponicSystemsComponent,
        AddAquaponicSystemComponent,
        AlertComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'aquaponics-systems', component: AquaponicSystemsComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
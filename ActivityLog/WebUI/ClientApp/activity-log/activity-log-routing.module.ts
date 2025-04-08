import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { ActivityLogComponent } from './activitylog/activitylog.component';
import { ActivityLogDetailsComponent } from './activitylog/activity-log-details/activity-log-details.component';
import { ActivityLogGroupCountComponent } from './activitylog/activity-log-group-count/activity-log-group-count.component';

const routes: Routes = [
  {
    path: 'activitylog',
    component: ActivityLogComponent,
    canActivate: [AuthorizeGuard]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class ActivityLogRoutingModule { }

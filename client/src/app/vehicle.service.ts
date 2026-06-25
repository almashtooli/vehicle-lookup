import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface Make { make_ID: number; make_Name: string; }
export interface VehicleType { vehicleTypeId: number; vehicleTypeName: string; }
export interface Model { model_ID: number; model_Name: string; }

@Injectable({ providedIn: 'root' })
export class VehicleService {
    private http = inject(HttpClient);

    getMakes(): Observable<Make[]> {
        return this.http.get<Make[]>('/api/makes');
    }

    getVehicleTypes(makeId: number): Observable<VehicleType[]> {
        return this.http.get<VehicleType[]>(`/api/makes/${makeId}/vehicle-types`);
    }

    getModels(makeId: number, year: number, vehicleType?: string): Observable<Model[]> {
        let url = `/api/makes/${makeId}/models?year=${year}`;
        if (vehicleType) url += `&vehicleType=${encodeURIComponent(vehicleType)}`;
        return this.http.get<Model[]>(url);
    }
}
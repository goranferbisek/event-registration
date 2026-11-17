import { data } from 'react-router'

const API_URL = import.meta.env.VITE_API_URL

export interface EventSummary {
  id: number
  name: string
  startDateTime: string
  location: string
  capacity: number
  registrationCount: number
}

export interface EventDetails {
  id: number
  name: string
  startDateTime: string
  location: string
  capacity: number
  freeSpots: number
}

export interface RegistrationRequest {
  fullName: string
  email: string
}

export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  status: number
  problem: ProblemDetails

  constructor(status: number, problem: ProblemDetails) {
    super(problem.title ?? `Request failed with status ${status}`)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
  }
}

async function readProblem(res: Response): Promise<ProblemDetails> {
  try {
    return await res.json()
  } catch {
    return {}
  }
}

async function send(path: string, init?: RequestInit): Promise<Response> {
  const res = await fetch(`${API_URL}${path}`, init)
  if (!res.ok) throw new ApiError(res.status, await readProblem(res))
  return res
}

export async function getEvents(from?: string, to?: string): Promise<EventSummary[]> {
  const params = new URLSearchParams()
  if (from) params.set('from', from)
  if (to) params.set('to', to)
  const query = params.toString()
  return (await send(`/events${query ? `?${query}` : ''}`)).json()
}

export async function getEvent(id: number): Promise<EventDetails> {
  return (await send(`/events/${id}`)).json()
}

export async function createRegistration(eventId: number, body: RegistrationRequest): Promise<void> {
  await send(`/events/${eventId}/registrations`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
}

export async function loadEvent(id: number): Promise<EventDetails> {
  if (!Number.isInteger(id)) throw data('Event not found', { status: 404 })
  try {
    return await getEvent(id)
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) throw data('Event not found', { status: 404 })
    throw error
  }
}

export function registrationClosedReason(event: EventDetails): string | null {
  if (new Date(event.startDateTime) <= new Date()) return 'Registration closed'
  if (event.freeSpots === 0) return 'Event is full'
  return null
}
